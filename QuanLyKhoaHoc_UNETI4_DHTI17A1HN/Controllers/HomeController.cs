// ========================================================
// Họ và tên: Nhóm 4 (Vũ Đình Trường - Nguyễn Đình Kiên - Nguyễn Đức Linh - Nguyễn Hoàng Long - Nguyễn Tùng Dương)
// Lớp: DHTI17A1HN - Đại học Kinh tế - Kỹ thuật Công nghiệp (UNETI)
// Nội dung thực hiện: Trang chủ Nền tảng Thương mại Giáo dục UNETI EDUSPACE (EdTech Commercial Homepage)
// ========================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
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
            var chuongTrinhs = await _context.ChuongTrinhDaoTaos
                .AsNoTracking()
                .Where(c => c.TrangThai)
                .ToListAsync();

            var openClassCounts = await _context.LopHocs
                .Where(l => l.TrangThai == "Đang mở đăng ký")
                .GroupBy(l => l.MaChuongTrinh)
                .Select(g => new { MaChuongTrinh = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaChuongTrinh, x => x.Count);

            ViewBag.ChuongTrinhs = chuongTrinhs;
            ViewBag.OpenClassCounts = openClassCounts;

            // 2. Các khóa học nổi bật / Bán chạy nhất (Top 6 lớp)
            var hotCourses = await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Include(l => l.GiangVien)
                .AsNoTracking()
                .Where(l => l.TrangThai == "Đang mở đăng ký")
                .OrderByDescending(l => l.MaLopHoc)
                .Take(6)
                .ToListAsync();

            // 3. Các khóa học sắp khai giảng (Top 4 lớp)
            var upcomingCourses = await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Include(l => l.GiangVien)
                .AsNoTracking()
                .Where(l => l.TrangThai == "Đang mở đăng ký" && l.NgayKhaiGiang >= DateTime.Today)
                .OrderBy(l => l.NgayKhaiGiang)
                .Take(4)
                .ToListAsync();

            // Sĩ số đã duyệt cho từng lớp hiển thị
            var allCourseIds = hotCourses.Select(c => c.MaLopHoc).Union(upcomingCourses.Select(c => c.MaLopHoc)).Distinct().ToList();
            var siSoMap = await _context.DangKyHocs
                .Where(d => allCourseIds.Contains(d.MaLopHoc) && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"))
                .GroupBy(d => d.MaLopHoc)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            ViewBag.HotCourses = hotCourses;
            ViewBag.UpcomingCourses = upcomingCourses;
            ViewBag.SiSoMap = siSoMap;

            // Lấy tất cả lớp học đang mở đăng ký cho tab Khóa học mở đăng ký
            var allOpenClasses = await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Include(l => l.GiangVien)
                .AsNoTracking()
                .Where(l => l.TrangThai == "Đang mở đăng ký")
                .OrderBy(l => l.MaLopHoc)
                .ToListAsync();
            ViewBag.AllOpenClasses = allOpenClasses;

            // Sĩ số cho tất cả các lớp đang mở
            var openIds = allOpenClasses.Select(c => c.MaLopHoc).ToList();
            var allSiSoMap = await _context.DangKyHocs
                .Where(d => openIds.Contains(d.MaLopHoc) && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"))
                .GroupBy(d => d.MaLopHoc)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);
            ViewBag.AllSiSoMap = allSiSoMap;

            // Danh sách giảng viên thực tế từ CSDL
            var giangViens = await _context.GiangViens
                .AsNoTracking()
                .Where(g => g.TrangThai)
                .OrderBy(g => g.MaGiangVien)
                .ToListAsync();
            ViewBag.GiangViens = giangViens;

            // 4. Thống kê số liệu thực tế của trung tâm
            ViewBag.TotalCourses = await _context.LopHocs.CountAsync();
            ViewBag.TotalStudents = await _context.HocViens.CountAsync();
            ViewBag.TotalRegistrations = await _context.DangKyHocs.CountAsync(d => d.TrangThai != "Đã hủy");

            // 5. Kiểm tra học viên đang đăng nhập (Personalized Learning Widget)
            int? maHocVien = HttpContext.Session.GetInt32("MaHocVien");
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            HocVien? student = null;
            if (vaiTro == "HocVien" && maHocVien.HasValue)
            {
                student = await _context.HocViens.FindAsync(maHocVien.Value);
            }
            else if (vaiTro == "HocVien" && maTaiKhoan.HasValue)
            {
                student = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);
            }

            ViewBag.CurrentStudent = (object?)student;
            var myRegistrations = new Dictionary<int, DangKyHoc>();
            BuoiHoc? nextSession = null;
            DangKyHoc? activeCourse = null;
            int totalSessions = 0;
            int completedSessions = 0;

            if (student != null)
            {
                var dks = await _context.DangKyHocs
                    .Include(d => d.LopHoc)
                        .ThenInclude(l => l!.ChuongTrinhDaoTao)
                    .Include(d => d.KetQuaHocTap)
                    .Where(d => d.MaHocVien == student.MaHocVien && d.TrangThai != "Đã hủy")
                    .OrderByDescending(d => d.NgayDangKy)
                    .ToListAsync();

                myRegistrations = dks.GroupBy(d => d.MaLopHoc)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(d => d.MaDangKy).First());

                activeCourse = dks.FirstOrDefault(d => d.TrangThai == "Đang học" || d.TrangThai == "Đã đóng học phí")
                             ?? dks.FirstOrDefault();

                if (activeCourse != null)
                {
                    nextSession = await _context.BuoiHocs
                        .Where(b => b.MaLopHoc == activeCourse.MaLopHoc && b.ThoiGianBatDau >= DateTime.Now && b.TrangThai != "Đã hủy")
                        .OrderBy(b => b.ThoiGianBatDau)
                        .FirstOrDefaultAsync();

                    totalSessions = activeCourse.LopHoc?.SoBuoiHoc ?? 1;
                    completedSessions = await _context.BuoiHocs
                        .CountAsync(b => b.MaLopHoc == activeCourse.MaLopHoc && b.TrangThai == "Đã hoàn thành");
                }

                // Lịch học / Thời khóa biểu cho các lớp sinh viên tham gia
                var enrolledClassIds = dks.Select(d => d.MaLopHoc).Distinct().ToList();
                var studentBuoiHocs = await _context.BuoiHocs
                    .Include(b => b.LopHoc)
                    .Where(b => enrolledClassIds.Contains(b.MaLopHoc) && b.TrangThai != "Đã hủy")
                    .OrderBy(b => b.ThoiGianBatDau)
                    .ToListAsync();

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
