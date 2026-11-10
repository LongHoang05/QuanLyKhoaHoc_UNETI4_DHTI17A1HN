using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    // ========================================================
    // Bổ sung theo yêu cầu giảng viên hướng dẫn:
    // Controller Quản lý Giảng viên & Cổng Giảng viên (Portal)
    // Áp dụng chuẩn kiến trúc Repository & Service Pattern
    // Phụ trách: Lịch giảng dạy, Phân công lớp & Cho điểm học viên
    // ========================================================
    [SessionAuthorize("Admin", "GiaoVu", "GiangVien")]
    public class GiangVienController : Controller
    {
        private readonly IGiangVienService _giangVienService;
        private readonly IGiangVienRepository _giangVienRepo;

        public GiangVienController(IGiangVienService giangVienService, IGiangVienRepository giangVienRepo)
        {
            _giangVienService = giangVienService;
            _giangVienRepo = giangVienRepo;
        }

        #region Helper xác định Giảng viên hiện tại từ Session
        private async Task<GiangVien?> GetCurrentGiangVienAsync(int? overrideMaGiangVien = null)
        {
            string? vaiTro = HttpContext.Session.GetString("VaiTro");
            bool isLecturerRole = string.Equals(vaiTro, "GiangVien", StringComparison.OrdinalIgnoreCase);

            // Nếu Admin/Giáo vụ truyền maGiangVien trên URL (ví dụ: bấm "Xem lịch dạy" từ Quản lý Giảng viên)
            if (!isLecturerRole && overrideMaGiangVien.HasValue && overrideMaGiangVien.Value > 0)
            {
                var targetGv = await _giangVienService.GetGiangVienByIdAsync(overrideMaGiangVien.Value);
                if (targetGv != null) return targetGv;
            }

            int? maGiangVien = isLecturerRole ? HttpContext.Session.GetInt32("MaGiangVien") : null;
            int? maTaiKhoan = isLecturerRole ? HttpContext.Session.GetInt32("MaTaiKhoan") : null;

            var gv = await _giangVienService.GetCurrentGiangVienAsync(maGiangVien, maTaiKhoan);
            if (gv != null && isLecturerRole)
            {
                if (!maGiangVien.HasValue)
                {
                    HttpContext.Session.SetInt32("MaGiangVien", gv.MaGiangVien);
                }
                HttpContext.Session.SetString("HoTen", gv.HoTen);
            }
            return gv;
        }
        #endregion

        // ========================================================
        // 1. DASHBOARD CỔNG GIẢNG VIÊN (Trang chủ giảng viên)
        // ========================================================
        public async Task<IActionResult> Index(int? maGiangVien = null)
        {
            var gv = await GetCurrentGiangVienAsync(maGiangVien);
            if (gv == null)
            {
                TempData["Error"] = "Chưa có thông tin giảng viên trong hệ thống.";
                return RedirectToAction("Index", "Home");
            }

            // Gọi qua Service Layer
            var myClasses = await _giangVienService.GetClassesForTeacherAsync(gv);
            var myClassIds = myClasses.Select(l => l.MaLopHoc).ToList();
            var upcomingSessions = await _giangVienService.GetUpcomingSessionsAsync(gv, 6);

            // Tổng học viên đang dạy
            int totalStudents = myClasses.Sum(l => l.DangKyHocs.Count(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học" || d.TrangThai == "Hoàn thành" || d.TrangThai == "Không hoàn thành"));

            // Số lượt học viên đã có điểm tổng kết
            int gradedCount = await _giangVienService.GetGradedCountAsync(myClassIds);

            ViewBag.CurrentGiangVien = gv;
            ViewBag.MyClasses = myClasses;
            ViewBag.UpcomingSessions = upcomingSessions;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.GradedCount = gradedCount;

            return View(gv);
        }

        // ========================================================
        // 2. XEM LỊCH DẠY / THỜI KHÓA BIỂU CỦA GIẢNG VIÊN
        // ========================================================
        public async Task<IActionResult> LichDay(int? maLop, DateTime? weekStart, int? maGiangVien = null)
        {
            var gv = await GetCurrentGiangVienAsync(maGiangVien);
            if (gv == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ giảng viên.";
                return RedirectToAction("Index", "Home");
            }

            var myClasses = await _giangVienService.GetClassesForTeacherAsync(gv);

            int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime selectedWeek = weekStart ?? DateTime.Today.AddDays(-diff);

            // Gọi qua Service Layer
            var weekSessions = await _giangVienService.GetWeeklyTimetableAsync(gv, maLop, selectedWeek);

            ViewBag.CurrentGiangVien = gv;
            ViewBag.MyClasses = myClasses;
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedWeek = selectedWeek;
            ViewBag.TotalWeekSessions = weekSessions.Count;

            return View(weekSessions);
        }

        // ========================================================
        // 3. CHẤM ĐIỂM & ĐÁNH GIÁ HỌC VIÊN THEO LỚP
        // ========================================================
        public async Task<IActionResult> ChamDiem(int? maLop, int? maGiangVien = null)
        {
            var gv = await GetCurrentGiangVienAsync(maGiangVien);
            if (gv == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ giảng viên.";
                return RedirectToAction("Index", "Home");
            }

            var myClasses = await _giangVienService.GetClassesForTeacherAsync(gv);

            if (!myClasses.Any())
            {
                // Fallback nếu chưa phân công
                myClasses = (await _giangVienRepo.GetClassesByTeacherAsync(0, "")).Take(5).ToList();
            }

            int selectedClassId = maLop ?? myClasses.FirstOrDefault()?.MaLopHoc ?? 0;
            var currentClass = myClasses.FirstOrDefault(l => l.MaLopHoc == selectedClassId);

            // Danh sách học viên lớp học lấy qua Service
            var studentEnrollments = selectedClassId > 0
                ? await _giangVienService.GetStudentsForGradingAsync(selectedClassId)
                : new List<DangKyHoc>();

            ViewBag.CurrentGiangVien = gv;
            ViewBag.MyClasses = myClasses;
            ViewBag.CurrentClass = currentClass;
            ViewBag.SelectedClassId = selectedClassId;

            return View(studentEnrollments);
        }

        // ========================================================
        // 4. AJAX LƯU ĐIỂM HỌC VIÊN (Service & Repository Pattern)
        // ========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuDiemAjax(int maDangKy, double? diemCC, double? diemGK, double? diemCK, string? ghiChu)
        {
            var (success, message, data) = await _giangVienService.LuuDiemHocVienAsync(maDangKy, diemCC, diemGK, diemCK, ghiChu);

            if (!success || data == null)
            {
                return Json(new { success = false, message });
            }

            var prop = data.GetType().GetProperties();
            var dict = prop.ToDictionary(p => p.Name, p => p.GetValue(data));

            return Json(new
            {
                success = true,
                message,
                diemTongKet = dict.ContainsKey("diemTongKet") ? dict["diemTongKet"] : "-",
                diemHe4 = dict.ContainsKey("diemHe4") ? dict["diemHe4"] : "-",
                diemChu = dict.ContainsKey("diemChu") ? dict["diemChu"] : "-",
                xepLoai = dict.ContainsKey("xepLoai") ? dict["xepLoai"] : "-",
                ketQua = dict.ContainsKey("ketQua") ? dict["ketQua"] : "-"
            });
        }

        // ========================================================
        // 5. QUẢN LÝ GIẢNG VIÊN (Dành cho Admin & Giáo vụ)
        // ========================================================
        [SessionAuthorize("Admin", "GiaoVu")]
        public async Task<IActionResult> QuanLy(string? search, string? hocVi, int page = 1)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (string.Equals(vaiTro, "GiangVien", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Giảng viên không có quyền quản lý danh sách Giảng viên của trung tâm.";
                return RedirectToAction(nameof(Index));
            }
            if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }

            var list = await _giangVienService.GetDanhSachGiangVienAsync(search, hocVi);

            int pageSize = 6;
            int totalItems = list.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pagedList = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Search = search;
            ViewBag.HocVi = hocVi;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;

            return View(pagedList);
        }

        // --- AJAX CRUD CHO QUẢN LÝ GIẢNG VIÊN QUA SERVICE ---
        [HttpGet]
        [SessionAuthorize("Admin", "GiaoVu")]
        public async Task<IActionResult> GetGiangVienJson(int id)
        {
            var gv = await _giangVienService.GetGiangVienByIdAsync(id);
            if (gv == null) return Json(new { success = false, message = "Không tìm thấy giảng viên." });

            return Json(new
            {
                success = true,
                data = new
                {
                    maGiangVien = gv.MaGiangVien,
                    hoTen = gv.HoTen,
                    email = gv.Email,
                    soDienThoai = gv.SoDienThoai ?? "",
                    hocVi = gv.HocVi ?? "",
                    chuyenNganh = gv.ChuyenNganh ?? "",
                    gioiThieu = gv.GioiThieu ?? "",
                    trangThai = gv.TrangThai,
                    tenDangNhap = gv.TaiKhoan?.TenDangNhap ?? ""
                }
            });
        }

        [HttpGet]
        [SessionAuthorize("Admin", "GiaoVu")]
        public IActionResult Create()
        {
            return RedirectToAction(nameof(QuanLy), new { create = "true" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize("Admin", "GiaoVu")]
        public async Task<IActionResult> CreatePost(GiangVien gv, string? tenDangNhap, string? matKhau)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var (success, message) = await _giangVienService.TaoGiangVienAsync(gv, tenDangNhap, matKhau);

            if (isAjax) return Json(new { success, message });

            if (success) TempData["Success"] = message;
            else TempData["Error"] = message;

            return RedirectToAction(nameof(QuanLy));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize("Admin", "GiaoVu")]
        public async Task<IActionResult> EditPost(int maGiangVien, GiangVien gv)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var (success, message) = await _giangVienService.CapNhatGiangVienAsync(maGiangVien, gv);

            if (isAjax) return Json(new { success, message });

            if (success) TempData["Success"] = message;
            else TempData["Error"] = message;

            return RedirectToAction(nameof(QuanLy));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize("Admin", "GiaoVu")]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var (success, message) = await _giangVienService.XoaGiangVienAsync(id);

            if (isAjax) return Json(new { success, message });

            if (success) TempData["Success"] = message;
            else TempData["Error"] = message;

            return RedirectToAction(nameof(QuanLy));
        }

        // ========================================================
        // ACTION DELETE MULTIPLE: XÓA NHIỀU GIẢNG VIÊN
        // ========================================================
        [HttpPost]
        [SessionAuthorize("Admin", "GiaoVu")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int>? ids)
        {
            if (ids == null || !ids.Any())
                return Json(new { success = false, message = "Vui lòng chọn ít nhất 1 giảng viên để xóa." });

            int deleted = 0;
            int skipped = 0;

            foreach (var id in ids)
            {
                var (success, _) = await _giangVienService.XoaGiangVienAsync(id);
                if (success) deleted++;
                else skipped++;
            }

            if (deleted == 0 && skipped > 0)
            {
                return Json(new { success = false, message = $"Không thể xóa {skipped} giảng viên đã chọn do đang phụ trách lớp học." });
            }

            string msg = $"Đã xóa thành công {deleted} giảng viên.";
            if (skipped > 0)
            {
                msg += $" Bỏ qua {skipped} giảng viên do đang phụ trách lớp học.";
            }
            return Json(new { success = true, message = msg, deletedCount = deleted, skippedCount = skipped });
        }
    }
}
