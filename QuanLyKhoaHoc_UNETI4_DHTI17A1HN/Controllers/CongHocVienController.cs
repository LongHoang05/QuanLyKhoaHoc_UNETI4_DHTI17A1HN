// ========================================================
// Cổng Học Viên - Portal Khám phá, Đăng ký và Quản lý Học tập
// Tác giả: Vũ Đình Trường, Nguyễn Đình Kiên, Nguyễn Đức Linh, Nguyễn Hoàng Long, Nguyễn Tùng Dương
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    public class CongHocVienController : Controller
    {
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IDangKyHocRepository _dangKyHocRepo;
        private readonly IChuongTrinhDaoTaoRepository _ctdtRepo;
        private readonly IHocVienRepository _hocVienRepo;
        private readonly IBuoiHocRepository _buoiHocRepo;
        private readonly IKetQuaHocTapRepository _ketQuaRepo;
        private readonly ITaiKhoanRepository _taiKhoanRepo;

        public CongHocVienController(
            ILopHocRepository lopHocRepo,
            IDangKyHocRepository dangKyHocRepo,
            IChuongTrinhDaoTaoRepository ctdtRepo,
            IHocVienRepository hocVienRepo,
            IBuoiHocRepository buoiHocRepo,
            IKetQuaHocTapRepository ketQuaRepo,
            ITaiKhoanRepository taiKhoanRepo)
        {
            _lopHocRepo = lopHocRepo;
            _dangKyHocRepo = dangKyHocRepo;
            _ctdtRepo = ctdtRepo;
            _hocVienRepo = hocVienRepo;
            _buoiHocRepo = buoiHocRepo;
            _ketQuaRepo = ketQuaRepo;
            _taiKhoanRepo = taiKhoanRepo;
        }

        // Helper lấy học viên hiện tại theo phiên Session đã đăng nhập (Không mock bừa bãi)
        private async Task<HocVien?> GetCurrentHocVienAsync()
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaHocVien");
            if (maHocVien.HasValue)
            {
                var hv = await _hocVienRepo.GetByIdAsync(maHocVien.Value);
                if (hv != null) return hv;
            }

            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan.HasValue)
            {
                var hv = await _hocVienRepo.GetByMaTaiKhoanAsync(maTaiKhoan.Value);
                if (hv != null) return hv;
            }

            return null;
        }

        // --- TRANG CHỦ MARKETPLACE KHÓA HỌC (STOREFRONT EDU-COMMERCE) ---
        public async Task<IActionResult> DangKy(
            string? search,
            int? maChuongTrinh,
            string? trinhDo,
            string? mucGia,
            string? sapXep,
            int page = 1,
            int pageSize = 8)
        {
            var student = await GetCurrentHocVienAsync();
            ViewBag.CurrentStudent = student;

            decimal? minHocPhi = null;
            decimal? maxHocPhi = null;
            if (mucGia == "duoi_3tr")
            {
                maxHocPhi = 3000000;
            }
            else if (mucGia == "3tr_6tr")
            {
                minHocPhi = 3000000;
                maxHocPhi = 6000000;
            }
            else if (mucGia == "tren_6tr")
            {
                minHocPhi = 6000000;
            }

            string sortBy = sapXep switch
            {
                "gia_tang" => "hocphi_asc",
                "gia_giam" => "hocphi_desc",
                "khai_giang_som" => "khaigiang_asc",
                "han_dang_ky" => "han_asc",
                _ => ""
            };

            if (page < 1) page = 1;
            var (classes, totalItems) = await _lopHocRepo.GetPagedAsync(
                search,
                maChuongTrinh,
                trinhDo,
                "Đang mở đăng ký",
                null,
                minHocPhi,
                maxHocPhi,
                null,
                null,
                sortBy,
                page,
                pageSize);

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages)
            {
                page = totalPages;
                (classes, totalItems) = await _lopHocRepo.GetPagedAsync(
                    search, maChuongTrinh, trinhDo, "Đang mở đăng ký", null, minHocPhi, maxHocPhi, null, null, sortBy, page, pageSize);
            }

            var classIds = classes.Select(c => c.MaLopHoc).ToList();
            var siSoMap = await _dangKyHocRepo.GetSiSoDaDangKyMapAsync(classIds);

            // Kiểm tra các lớp mà học viên hiện tại đã đăng ký
            var myRegistrations = new Dictionary<int, DangKyHoc>();
            if (student != null)
            {
                var dks = await _dangKyHocRepo.GetByHocVienIdAsync(student.MaHocVien);
                myRegistrations = dks
                    .Where(d => d.TrangThai != "Đã hủy" && d.TrangThai != "Từ chối")
                    .GroupBy(d => d.MaLopHoc)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(d => d.MaDangKy).First());
            }

            // Marketplace Stats
            ViewBag.TongKhoaHoc = await _lopHocRepo.CountOpenClassesAsync();
            ViewBag.TongHocVien = await _hocVienRepo.CountTotalStudentsAsync();
            ViewBag.TongChuongTrinh = (await _ctdtRepo.GetAllAsync()).Count;

            ViewBag.SiSoMap = siSoMap;
            ViewBag.MyRegistrations = myRegistrations;
            ViewBag.ChuongTrinhList = await _ctdtRepo.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.SelectedMaChuongTrinh = maChuongTrinh;
            ViewBag.SelectedTrinhDo = trinhDo;
            ViewBag.SelectedMucGia = mucGia;
            ViewBag.SelectedSapXep = sapXep;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;

            return View(classes);
        }

        // --- TRANG LANDING PAGE CHI TIẾT KHÓA HỌC (COURSE DETAILS & STICKY CTA) ---
        public async Task<IActionResult> ChiTiet(int id)
        {
            var student = await GetCurrentHocVienAsync();
            ViewBag.CurrentStudent = student;

            var lop = await _lopHocRepo.GetByIdWithDetailsAsync(id);
            if (lop == null) return NotFound();

            // Danh sách buổi học chi tiết
            var buoiHocs = (await _buoiHocRepo.GetByLopHocIdAsync(id))
                .Where(b => b.TrangThai != "Đã hủy")
                .OrderBy(b => b.ThoiGianBatDau)
                .ToList();

            // Sĩ số đã tiếp nhận
            int enrolledCount = await _dangKyHocRepo.CountEnrolledInLopAsync(id);

            // Đơn đăng ký của học viên hiện tại nếu có
            DangKyHoc? myReg = null;
            if (student != null)
            {
                myReg = await _dangKyHocRepo.GetActiveRegistrationAsync(student.MaHocVien, id);
            }

            // Các khóa học tương tự cùng chương trình đào tạo
            var similarCourses = await _lopHocRepo.GetSimilarCoursesAsync(lop.MaChuongTrinh, id, 3);

            ViewBag.BuoiHocs = buoiHocs;
            ViewBag.EnrolledCount = enrolledCount;
            ViewBag.RemainingSeats = Math.Max(0, lop.SiSoToiDa - enrolledCount);
            ViewBag.PercentFilled = lop.SiSoToiDa > 0 ? Math.Min(100.0, Math.Round((double)enrolledCount / lop.SiSoToiDa * 100, 1)) : 0;
            ViewBag.MyRegistration = myReg;
            ViewBag.SimilarCourses = similarCourses;

            return View(lop);
        }

        // --- HÀNH ĐỘNG ĐĂNG KÝ LỚP HỌC (AJAX) - TUÂN THỦ MỤC 7.3 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyLopPost(int maLop, int? maLopHoc, string? ghiChu)
        {
            int targetMaLop = maLop > 0 ? maLop : (maLopHoc ?? 0);

            // 1. Kiểm tra học viên đã đăng nhập
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập tài khoản học viên để thực hiện đăng ký." });
            }

            // 2. Kiểm tra học viên đang hoạt động
            if (!student.TrangThai)
            {
                return Json(new { success = false, message = "Tài khoản học viên của bạn đang bị khóa, không thể đăng ký khóa học." });
            }

            // 3. Kiểm tra lớp học tồn tại và ở trạng thái 'Đang mở đăng ký'
            var lop = await _lopHocRepo.GetByIdAsync(targetMaLop);
            if (lop == null)
            {
                return Json(new { success = false, message = "Lớp học không tồn tại." });
            }

            if (lop.TrangThai != "Đang mở đăng ký")
            {
                return Json(new { success = false, message = "Lớp học này hiện không mở tuyển sinh." });
            }

            // 4. Kiểm tra ngày hiện tại nằm trong thời gian mở đăng ký của lớp
            if (DateTime.Today < lop.NgayBatDauDangKy.Date)
            {
                return Json(new { success = false, message = $"Lớp học chưa đến ngày mở đăng ký ({lop.NgayBatDauDangKy:dd/MM/yyyy})." });
            }

            if (DateTime.Today > lop.HanDangKy.Date)
            {
                return Json(new { success = false, message = "Lớp học đã hết thời hạn nhận đăng ký." });
            }

            // 5. Kiểm tra học viên không được có hai đăng ký đang xử lý cho cùng một lớp
            bool daDangKy = await _dangKyHocRepo.HasEnrolledAsync(student.MaHocVien, targetMaLop);
            if (daDangKy)
            {
                return Json(new { success = false, message = "Bạn đã có hồ sơ đăng ký cho khóa học này rồi." });
            }

            // 6. Kiểm tra lớp còn chỗ theo sĩ số tối đa
            int enrolledCount = await _dangKyHocRepo.CountEnrolledInLopAsync(targetMaLop);
            if (enrolledCount >= lop.SiSoToiDa)
            {
                return Json(new { success = false, message = "Khóa học đã đủ chỉ tiêu sĩ số. Vui lòng chọn lớp học khác." });
            }

            // 7. Tạo đăng ký mới với trạng thái 'Chờ duyệt', ngày do hệ thống xác định, học phí 0
            var dangKy = new DangKyHoc
            {
                MaHocVien = student.MaHocVien,
                MaLopHoc = targetMaLop,
                NgayDangKy = DateTime.Now,
                TrangThai = "Chờ duyệt",
                GhiChu = ghiChu ?? "",
                SoTienDaDong = 0
            };

            await _dangKyHocRepo.AddAsync(dangKy);

            return Json(new
            {
                success = true,
                message = $"Đăng ký thành công khóa học '{lop.TenLop}'! Đơn của bạn đang ở trạng thái 'Chờ duyệt'."
            });
        }

        // --- HÀNH ĐỘNG HỦY ĐĂNG KÝ LỚP HỌC (AJAX) - TUÂN THỦ MỤC 7.4 ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKyPost(int maDangKy)
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null) return Json(new { success = false, message = "Bạn cần đăng nhập để thao tác." });

            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(maDangKy);
            if (dk == null) return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });

            // Quy định: Học viên chỉ được hủy đăng ký của chính mình
            if (dk.MaHocVien != student.MaHocVien)
            {
                return Json(new { success = false, message = "Bạn không có quyền thao tác trên hồ sơ của người khác." });
            }

            // Quy định: Chỉ được hủy trong trạng thái Chờ duyệt hoặc Đã duyệt nhưng chưa đóng học phí
            if (dk.TrangThai != "Chờ duyệt" && !(dk.TrangThai == "Đã duyệt" && dk.SoTienDaDong == 0))
            {
                return Json(new { success = false, message = $"Không thể tự hủy đơn khi đã ở trạng thái '{dk.TrangThai}'. Vui lòng liên hệ ban giáo vụ để được hỗ trợ giải quyết." });
            }

            dk.TrangThai = "Đã hủy";
            await _dangKyHocRepo.UpdateAsync(dk);

            return Json(new { success = true, message = $"Đã hủy đăng ký lớp '{dk.LopHoc?.TenLop}' thành công." });
        }

        // --- HỌC VIÊN XÁC NHẬN NỘP HỌC PHÍ (FORM POST THUẦN MVC) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToanHocPhi(int maDangKy, string? ghiChu)
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(maDangKy);
            if (dk != null && dk.MaHocVien == student.MaHocVien)
            {
                decimal hocPhiLop = dk.LopHoc?.HocPhi ?? 0m;

                // Cập nhật nộp đủ 100% học phí
                dk.SoTienDaDong = hocPhiLop;
                dk.NgayDongHocPhi = DateTime.Now;
                dk.TrangThai = "Đã đóng học phí";
                dk.GhiChu = string.IsNullOrWhiteSpace(ghiChu)
                    ? "Học viên xác nhận chuyển khoản 100% học phí"
                    : ghiChu.Trim();

                if (dk.NgayXuLy == null)
                {
                    dk.NgayXuLy = DateTime.Now;
                    dk.NhanXetGiaoVu = "Đã nộp học phí";
                }

                await _dangKyHocRepo.UpdateAsync(dk);
                TempData["Success"] = $"Đã xác nhận nộp học phí lớp '{dk.LopHoc?.TenLop}' thành công!";
            }

            return RedirectToAction(nameof(TienTrinh));
        }

        // --- KHÔNG GIAN HỌC TẬP CỦA TÔI (MY LEARNING DASHBOARD) ---
        public async Task<IActionResult> TienTrinh(string? tab = null, DateTime? weekStart = null, int? maLop = null)
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem không gian học tập.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            ViewBag.CurrentStudent = student;

            if ((weekStart.HasValue || maLop.HasValue) && string.IsNullOrEmpty(tab))
            {
                tab = "schedule";
            }

            // Ánh xạ tham số tab trên URL từ menu người dùng sang tab tương ứng
            string activeTab = (tab ?? "").Trim().ToLower() switch
            {
                "tkb" or "schedule" => "schedule",
                "ketqua" or "grades" => "grades",
                "hoso" or "profile" => "profile",
                _ => "courses"
            };
            ViewBag.ActiveTab = activeTab;

            // Thông tin tài khoản & Lịch sử đóng học phí cho Tab Thẻ sinh viên & Hồ sơ
            var tk = student.MaTaiKhoan.HasValue ? await _taiKhoanRepo.GetByIdAsync(student.MaTaiKhoan.Value) : null;
            ViewBag.TaiKhoan = tk;
            var paymentHistory = await _dangKyHocRepo.GetPaymentHistoryByHocVienIdAsync(student.MaHocVien);
            ViewBag.PaymentHistory = paymentHistory;

            // Danh sách các đăng ký của chính học viên này
            var dangKys = (await _dangKyHocRepo.GetByHocVienIdAsync(student.MaHocVien)).ToList();

            // Danh sách các lớp học viên đang học hoặc đã đóng phí để lấy lịch học
            var activeClassIds = dangKys
                .Where(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học")
                .Select(d => d.MaLopHoc)
                .ToList();

            var allBuoiHocs = await _buoiHocRepo.GetBuoiHocsByLopIdsAsync(activeClassIds);
            var buoiHocs = allBuoiHocs
                .Where(b => b.TrangThai != "Đã hủy")
                .OrderBy(b => b.ThoiGianBatDau)
                .ToList();

            // Lớp học của tôi phục vụ bộ lọc thời khóa biểu
            var myClasses = dangKys
                .Where(d => d.LopHoc != null && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"))
                .Select(d => d.LopHoc!)
                .GroupBy(l => l.MaLopHoc)
                .Select(g => g.First())
                .ToList();
            ViewBag.MyClasses = myClasses;

            // Tính tuần học (Thứ 2 đầu tuần)
            int diffWeek = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime currentWeekStart = weekStart ?? DateTime.Today.AddDays(-diffWeek);
            ViewBag.SelectedWeek = currentWeekStart;
            ViewBag.SelectedMaLop = maLop;

            // Lọc theo lớp nếu có
            if (maLop.HasValue && maLop.Value > 0)
            {
                buoiHocs = buoiHocs.Where(b => b.MaLopHoc == maLop.Value).ToList();
            }

            DateTime weekEnd = currentWeekStart.AddDays(7);
            int totalWeekSessions = buoiHocs.Count(b => b.ThoiGianBatDau >= currentWeekStart && b.ThoiGianBatDau < weekEnd);
            ViewBag.TotalWeekSessions = totalWeekSessions;

            // Đếm số buổi đã hoàn thành cho từng lớp để tính tiến độ %
            var completedSessionsCount = buoiHocs
                .Where(b => b.TrangThai == "Đã hoàn thành")
                .GroupBy(b => b.MaLopHoc)
                .ToDictionary(g => g.Key, g => g.Count());

            // Bảng điểm cá nhân
            var ketQuas = (await _ketQuaRepo.GetByHocVienIdAsync(student.MaHocVien)).ToList();

            ViewBag.BuoiHocs = buoiHocs;
            ViewBag.CompletedSessionsCount = completedSessionsCount;
            ViewBag.KetQuas = ketQuas;

            return View(dangKys);
        }

        // --- HỒ SƠ CÁ NHÂN HỌC VIÊN ---
        public async Task<IActionResult> HoSo()
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem hồ sơ.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var tk = student.MaTaiKhoan.HasValue ? await _taiKhoanRepo.GetByIdAsync(student.MaTaiKhoan.Value) : null;
            ViewBag.TaiKhoan = tk;

            // Lịch sử thanh toán học phí
            var paymentHistory = await _dangKyHocRepo.GetPaymentHistoryByHocVienIdAsync(student.MaHocVien);
            ViewBag.PaymentHistory = paymentHistory;

            return View(student);
        }

        // --- CẬP NHẬT HỒ SƠ CÁ NHÂN ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatHoSoPost(HocVien model)
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null || (model.MaHocVien > 0 && student.MaHocVien != model.MaHocVien))
            {
                return Json(new { success = false, message = "Bạn không có quyền chỉnh sửa hồ sơ này." });
            }

            if (string.IsNullOrWhiteSpace(model.HoTen))
            {
                return Json(new { success = false, message = "Họ và tên học viên không được để trống." });
            }

            if (model.NgaySinh.HasValue && model.NgaySinh.Value.Date > DateTime.Today)
            {
                return Json(new { success = false, message = "Ngày sinh không được lớn hơn ngày hiện tại." });
            }

            if (!string.IsNullOrWhiteSpace(model.SoDienThoai) && !System.Text.RegularExpressions.Regex.IsMatch(model.SoDienThoai.Trim(), @"^0\d{9}$"))
            {
                return Json(new { success = false, message = "Số điện thoại phải gồm đúng 10 chữ số và bắt đầu bằng số 0 (Ví dụ: 0912345678)." });
            }

            student.HoTen = model.HoTen.Trim();
            if (model.NgaySinh.HasValue) student.NgaySinh = model.NgaySinh;
            if (!string.IsNullOrWhiteSpace(model.GioiTinh)) student.GioiTinh = model.GioiTinh;
            if (model.SoDienThoai != null) student.SoDienThoai = model.SoDienThoai.Trim();
            if (model.DiaChi != null) student.DiaChi = model.DiaChi.Trim();
            if (!string.IsNullOrWhiteSpace(model.TrinhDoHocVan)) student.TrinhDoHocVan = model.TrinhDoHocVan;
            if (model.NgheNghiep != null) student.NgheNghiep = model.NgheNghiep.Trim();

            await _hocVienRepo.UpdateAsync(student);

            // Đồng bộ tên lên Session & Tài khoản để thanh điều hướng cập nhật ngay
            HttpContext.Session.SetString("HoTen", student.HoTen);

            if (student.MaTaiKhoan.HasValue)
            {
                var tk = await _taiKhoanRepo.GetByIdAsync(student.MaTaiKhoan.Value);
                if (tk != null)
                {
                    tk.HoTen = student.HoTen;
                    await _taiKhoanRepo.UpdateAsync(tk);
                }
            }

            return Json(new { success = true, message = "Cập nhật thông tin cá nhân thành công!" });
        }
    }
}
