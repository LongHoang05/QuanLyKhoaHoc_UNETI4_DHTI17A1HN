// ========================================================
// Họ và tên: Nhóm 4 (Vũ Đình Trường - Nguyễn Đình Kiên - Nguyễn Đức Linh - Nguyễn Hoàng Long - Nguyễn Tùng Dương)
// Lớp: DHTI17A1HN - Đại học Kinh tế - Kỹ thuật Công nghiệp (UNETI)
// Nội dung thực hiện: Trang chủ Nền tảng Thương mại Giáo dục UNETI EDUSPACE (EdTech Commercial Homepage)
// ========================================================

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    public class HomeController : Controller
    {
        private readonly IChuongTrinhDaoTaoRepository _ctdtRepo;
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IDangKyHocRepository _dangKyHocRepo;
        private readonly IGiangVienRepository _giangVienRepo;
        private readonly IHocVienRepository _hocVienRepo;
        private readonly IBuoiHocRepository _buoiHocRepo;

        public HomeController(
            IChuongTrinhDaoTaoRepository ctdtRepo,
            ILopHocRepository lopHocRepo,
            IDangKyHocRepository dangKyHocRepo,
            IGiangVienRepository giangVienRepo,
            IHocVienRepository hocVienRepo,
            IBuoiHocRepository buoiHocRepo)
        {
            _ctdtRepo = ctdtRepo;
            _lopHocRepo = lopHocRepo;
            _dangKyHocRepo = dangKyHocRepo;
            _giangVienRepo = giangVienRepo;
            _hocVienRepo = hocVienRepo;
            _buoiHocRepo = buoiHocRepo;
        }

        // --- TRANG CHỦ NỀN TẢNG THƯƠNG MẠI GIÁO DỤC (EDTECH STOREFRONT HOME) ---
        public async Task<IActionResult> Index(bool storefront = false)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!storefront && (vaiTro == "Admin" || vaiTro == "GiaoVu"))
            {
                return RedirectToAction("Dashboard", "ThongKe");
            }
            if (!storefront && vaiTro == "GiangVien")
            {
                return RedirectToAction("Index", "GiangVien");
            }

            // 1. Danh sách chương trình đào tạo & đếm số lớp đang mở
            var allCtdt = await _ctdtRepo.GetAllAsync();
            var chuongTrinhs = allCtdt.Where(c => c.TrangThai).ToList();
            var openClassCounts = await _lopHocRepo.GetOpenClassCountsByProgramAsync();

            ViewBag.ChuongTrinhs = chuongTrinhs;
            ViewBag.OpenClassCounts = openClassCounts;

            // 2. Các khóa học nổi bật / Bán chạy nhất (Top 6 lớp)
            var hotCourses = (await _lopHocRepo.GetHotCoursesAsync(6)).ToList();

            // 3. Các khóa học sắp khai giảng (Top 4 lớp)
            var upcomingCourses = (await _lopHocRepo.GetUpcomingCoursesAsync(4)).ToList();

            // Sĩ số đã duyệt cho từng lớp hiển thị
            var allCourseIds = hotCourses.Select(c => c.MaLopHoc).Union(upcomingCourses.Select(c => c.MaLopHoc)).Distinct().ToList();
            var siSoMap = await _dangKyHocRepo.GetSiSoDaDangKyMapAsync(allCourseIds);

            ViewBag.HotCourses = hotCourses;
            ViewBag.UpcomingCourses = upcomingCourses;
            ViewBag.SiSoMap = siSoMap;

            // Lấy tất cả lớp học đang mở đăng ký cho tab Khóa học mở đăng ký
            var allLops = await _lopHocRepo.GetAllAsync();
            var allOpenClasses = allLops.Where(l => l.TrangThai == "Đang mở đăng ký").OrderBy(l => l.MaLopHoc).ToList();
            ViewBag.AllOpenClasses = allOpenClasses;

            // Sĩ số cho tất cả các lớp đang mở
            var openIds = allOpenClasses.Select(c => c.MaLopHoc).ToList();
            var allSiSoMap = await _dangKyHocRepo.GetSiSoDaDangKyMapAsync(openIds);
            ViewBag.AllSiSoMap = allSiSoMap;

            // Danh sách giảng viên thực tế từ CSDL
            var allTeachers = await _giangVienRepo.GetAllAsync();
            var giangViens = allTeachers.Where(g => g.TrangThai).OrderBy(g => g.MaGiangVien).ToList();
            ViewBag.GiangViens = giangViens;

            // 4. Thống kê số liệu thực tế của trung tâm
            ViewBag.TotalCourses = await _lopHocRepo.CountTotalClassesAsync();
            ViewBag.TotalStudents = await _hocVienRepo.CountTotalStudentsAsync();
            ViewBag.TotalRegistrations = await _dangKyHocRepo.GetTotalCountExcludingCancelledAsync();

            // 5. Kiểm tra học viên đang đăng nhập (Personalized Learning Widget)
            int? maHocVien = HttpContext.Session.GetInt32("MaHocVien");
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            HocVien? student = null;
            if (vaiTro == "HocVien" && maHocVien.HasValue)
            {
                student = await _hocVienRepo.GetByIdAsync(maHocVien.Value);
            }
            else if (vaiTro == "HocVien" && maTaiKhoan.HasValue)
            {
                student = await _hocVienRepo.GetByMaTaiKhoanAsync(maTaiKhoan.Value);
            }

            ViewBag.CurrentStudent = (object?)student;
            var myRegistrations = new Dictionary<int, DangKyHoc>();
            BuoiHoc? nextSession = null;
            DangKyHoc? activeCourse = null;
            int totalSessions = 0;
            int completedSessions = 0;

            if (student != null)
            {
                var allDks = await _dangKyHocRepo.GetByHocVienIdAsync(student.MaHocVien);
                var dks = allDks.Where(d => d.TrangThai != "Đã hủy").ToList();

                myRegistrations = dks.GroupBy(d => d.MaLopHoc)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(d => d.MaDangKy).First());

                activeCourse = dks.FirstOrDefault(d => d.TrangThai == "Đang học" || d.TrangThai == "Đã đóng học phí")
                             ?? dks.FirstOrDefault();

                if (activeCourse != null)
                {
                    nextSession = await _buoiHocRepo.GetNextBuoiHocForLopAsync(activeCourse.MaLopHoc);
                    totalSessions = activeCourse.LopHoc?.SoBuoiHoc ?? 1;
                    completedSessions = await _buoiHocRepo.CountCompletedSessionsForLopAsync(activeCourse.MaLopHoc);
                }

                // Lịch học / Thời khóa biểu cho các lớp sinh viên tham gia
                var enrolledClassIds = dks.Select(d => d.MaLopHoc).Distinct().ToList();
                var allStudentSessions = await _buoiHocRepo.GetBuoiHocsByLopIdsAsync(enrolledClassIds);
                var studentBuoiHocs = allStudentSessions
                    .Where(b => b.TrangThai != "Đã hủy")
                    .OrderBy(b => b.ThoiGianBatDau)
                    .ToList();

                // Bảng điểm & Kết quả học tập thực tế từ CSDL
                var studentKetQuas = dks
                    .Where(d => d.KetQuaHocTap != null)
                    .Select(d => d.KetQuaHocTap!)
                    .ToList();

                double avgScore = studentKetQuas.Any() ? Math.Round(studentKetQuas.Average(k => (double)k.DiemTongKet), 1) : 0.0;
                double gpa4 = studentKetQuas.Any() ? Math.Round(studentKetQuas.Average(k => k.DiemHe4 ?? 0.0), 2) : 0.0;
                int chuyenCanPercent = totalSessions > 0 ? Math.Min(100, (int)Math.Round((double)completedSessions / totalSessions * 100)) : 0;

                ViewBag.StudentDangKys = dks;
                ViewBag.DangKyCount = dks.Count;
                ViewBag.LopDangHocCount = dks.Count(d => d.TrangThai == "Đang học" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đã duyệt");
                ViewBag.ChuyenCanPercent = chuyenCanPercent;
                ViewBag.StudentBuoiHocs = studentBuoiHocs;
                ViewBag.StudentKetQuas = studentKetQuas;
                ViewBag.AvgScore = avgScore;
                ViewBag.Gpa4 = gpa4;
            }
            else
            {
                ViewBag.StudentDangKys = new List<DangKyHoc>();
                ViewBag.StudentBuoiHocs = new List<BuoiHoc>();
                ViewBag.StudentKetQuas = new List<KetQuaHocTap>();
                ViewBag.DangKyCount = 0;
                ViewBag.LopDangHocCount = 0;
                ViewBag.ChuyenCanPercent = 0;
                ViewBag.AvgScore = 0.0;
                ViewBag.Gpa4 = 0.0;
            }

            ViewBag.MyRegistrations = myRegistrations;
            ViewBag.ActiveCourse = activeCourse;
            ViewBag.NextSession = nextSession;
            ViewBag.TotalSessions = totalSessions;
            ViewBag.CompletedSessions = completedSessions;

            return View();
        }

        // --- HÀNH ĐỘNG GỬI YÊU CẦU TƯ VẤN LỘ TRÌNH (AJAX) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuiTuVan(string hoTen, string soDienThoai, string? email, string? khoaHocQuanTam)
        {
            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(soDienThoai))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ họ tên và số điện thoại liên hệ." });
            }

            return Json(new
            {
                success = true,
                message = $"Cảm ơn bạn {hoTen}! Ban Tuyển sinh UNETI EDUSPACE sẽ liên hệ qua SĐT {soDienThoai} trong vòng 24 giờ để tư vấn lộ trình học phù hợp nhất."
            });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
