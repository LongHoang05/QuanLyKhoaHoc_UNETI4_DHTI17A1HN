// ========================================================
// Họ và tên: Nguyễn Đức Linh
// Mã sinh viên: 23103100042
// Phụ trách: Module 3 - Quản lý học viên, Hồ sơ cá nhân và Đăng ký học
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    public class HocVienController : Controller
    {
        private readonly IHocVienService _hocVienService;
        private readonly IHocVienRepository _hocVienRepo;
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IChuongTrinhDaoTaoRepository _ctdtRepo;

        public HocVienController(
            IHocVienService hocVienService,
            IHocVienRepository hocVienRepo,
            ILopHocRepository lopHocRepo,
            IChuongTrinhDaoTaoRepository ctdtRepo)
        {
            _hocVienService = hocVienService;
            _hocVienRepo = hocVienRepo;
            _lopHocRepo = lopHocRepo;
            _ctdtRepo = ctdtRepo;
        }

        #region Helper Phân Quyền Session (Theo Mục 5.4 & Mục 7)
        /// <summary>
        /// Xác thực vai trò người dùng trong Session: Chỉ Admin và GiaoVu mới có quyền quản lý toàn bộ học viên
        /// </summary>
        private bool CoQuyenQuanTri(out IActionResult? redirectResult)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");

            // 1. Chưa đăng nhập -> Chuyển về trang đăng nhập
            if (maTaiKhoan == null || string.IsNullOrEmpty(vaiTro))
            {
                var returnUrl = Request.Path + Request.QueryString;
                redirectResult = RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl });
                return false;
            }

            // 2. Đăng nhập nhưng vai trò không phải Admin hoặc GiaoVu -> Chặn quyền truy cập
            if (vaiTro != "Admin" && vaiTro != "GiaoVu")
            {
                TempData["Error"] = "Tài khoản học viên không có quyền quản lý danh sách học viên của trung tâm!";
                redirectResult = RedirectToAction("AccessDenied", "Home");
                return false;
            }

            redirectResult = null;
            return true;
        }
        #endregion

        // ========================================================
        // 1. ACTION INDEX: HIỂN THỊ GIAO DIỆN CHÍNH 2 TAB (TaiKhoan & HocVien)
        // ========================================================
        public async Task<IActionResult> Index(
            string? search = null,
            string? trangThai = null,
            string? trinhDo = null,
            string? sortBy = null,
            int page = 1)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            int pageSize = 8;
            if (page < 1) page = 1;

            bool? filterTrangThai = null;
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                if (trangThai == "Đang học" || trangThai == "active" || trangThai.ToLower() == "true")
                    filterTrangThai = true;
                else if (trangThai == "Tạm khóa" || trangThai == "locked" || trangThai.ToLower() == "false")
                    filterTrangThai = false;
            }

            var (pagedStudents, totalItems) = await _hocVienRepo.GetPagedAsync(
                search, filterTrangThai, page, pageSize, trinhDo, sortBy);

            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages)
            {
                page = totalPages;
                (pagedStudents, totalItems) = await _hocVienRepo.GetPagedAsync(
                    search, filterTrangThai, page, pageSize, trinhDo, sortBy);
            }

            var allStudents = await _hocVienService.GetDanhSachHocVienAsync();
            var accounts = await _hocVienService.GetDanhSachTaiKhoanAsync();
            var classes = await _hocVienService.GetDanhSachLopHocAsync();
            var (tongHocVien, soGiaoVu, soAdmin, soBiKhoa) = await _hocVienService.GetThongKeAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.DangHocCount = allStudents.Count(h => h.TrangThai);
            ViewBag.TamKhoaCount = allStudents.Count(h => !h.TrangThai);
            ViewBag.Search = search;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.SelectedTrinhDo = trinhDo;
            ViewBag.SortBy = sortBy;

            var viewModel = new HocVienIndexViewModel
            {
                DanhSachTaiKhoan = accounts,
                DanhSachHocVien = pagedStudents.ToList(),
                DanhSachLopHoc = classes,
                TongSoHocVien = tongHocVien,
                SoGiaoVu = soGiaoVu,
                SoAdmin = soAdmin,
                SoBiKhoa = soBiKhoa,
                CurrentTab = "profiles"
            };

            return View(viewModel);
        }

        // ========================================================
        // 2. AJAX FILTER HỌC VIÊN: TÌM KIẾM + TRẠNG THÁI + LỌC THEO LỚP HỌC
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> FilterHocVienAjax(string? keyword, string? status, int? maLopHoc)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Unauthorized();
            }

            bool? trangThai = null;
            if (status == "active") trangThai = true;
            else if (status == "locked") trangThai = false;

            var danhSach = await _hocVienService.TimKiemHocVienAsync(keyword, trangThai, maLopHoc);
            return PartialView("_DanhSachHocVien", danhSach);
        }

        // ========================================================
        // 3. AJAX FILTER TÀI KHOẢN: TÌM KIẾM + VAI TRÒ + TRẠNG THÁI
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> FilterTaiKhoanAjax(string? keyword, string? role, string? status)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Unauthorized();
            }

            bool? trangThai = null;
            if (status == "active") trangThai = true;
            else if (status == "locked") trangThai = false;

            string? vaiTro = null;
            if (role == "Admin") vaiTro = "Admin";
            else if (role == "Giáo vụ" || role == "GiaoVu") vaiTro = "GiaoVu";
            else if (role == "Giảng viên" || role == "GiangVien") vaiTro = "GiangVien";
            else if (role == "Học viên" || role == "HocVien") vaiTro = "HocVien";

            var danhSach = await _hocVienService.TimKiemTaiKhoanAsync(keyword, vaiTro, trangThai);
            return PartialView("_DanhSachTaiKhoan", danhSach);
        }

        // ========================================================
        // 4. AJAX LẤY CHI TIẾT HỌC VIÊN RENDER MODAL
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> GetChiTietHocVienJson(int id)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Json(new { success = false, message = "Bạn không có quyền xem thông tin học viên." });
            }

            var hv = await _hocVienService.GetChiTietHocVienAsync(id);
            if (hv == null)
            {
                return Json(new { success = false, message = "Không tìm thấy học viên." });
            }

            var tenCacLop = hv.DangKyHocs != null && hv.DangKyHocs.Any()
                ? string.Join(", ", hv.DangKyHocs.Where(d => d.LopHoc != null).Select(d => d.LopHoc!.TenLop))
                : "Chưa đăng ký lớp";

            var dto = new HocVienChiTietModalDto
            {
                MaHocVien = hv.MaHocVien,
                MaTaiKhoan = hv.MaTaiKhoan,
                HoTen = hv.HoTen,
                NgaySinh = hv.NgaySinh.HasValue ? hv.NgaySinh.Value.ToString("yyyy-MM-dd") : "Chưa cập nhật",
                GioiTinh = hv.GioiTinh ?? "Không rõ",
                SoDienThoai = hv.SoDienThoai ?? "Chưa có",
                Email = hv.Email,
                DiaChi = hv.DiaChi ?? "Chưa cập nhật",
                TrinhDoHocVan = hv.TrinhDoHocVan ?? "Chưa rõ",
                NgheNghiep = hv.NgheNghiep ?? "Chưa có",
                TrangThai = hv.TrangThai ? "Đang học" : "Đang bị khóa",
                LopHocDangKy = tenCacLop
            };

            return Json(new { success = true, data = dto });
        }

        // ========================================================
        // 5. AJAX KHÓA / MỞ KHÓA TÀI KHOẢN (Entity TaiKhoan - Mục 5.1)
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> ToggleTaiKhoanStatus(int id)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Json(new { success = false, message = "Bạn không có quyền quản trị." });
            }

            var (success, error, newStatus, username) = await _hocVienService.DoiTrangThaiTaiKhoanAsync(id);
            return Json(new { success, message = error, newStatus, username });
        }

        // ========================================================
        // 6. AJAX CẤP LẠI MẬT KHẨU TÀI KHOẢN
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> ResetMatKhau(int id, string? newPassword)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Json(new { success = false, message = "Bạn không có quyền quản trị." });
            }

            var (success, error) = await _hocVienService.ResetMatKhauTaiKhoanAsync(id, newPassword);
            return Json(new { success, message = error ?? "Cấp lại mật khẩu thành công!" });
        }

        // ========================================================
        // 7. AJAX THÊM TÀI KHOẢN MỚI TỪ MODAL
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> ThemNguoiDungAjax(ThemTaiKhoanViewModel model)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Json(new { success = false, message = "Bạn không có quyền quản trị." });
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errors });
            }

            var (success, error) = await _hocVienService.ThemTaiKhoanMoiAsync(model);
            return Json(new { success, message = error ?? $"Tạo tài khoản @{model.TenDangNhap} thành công!" });
        }

        // ========================================================
        // 8. AJAX ĐỔI TRẠNG THÁI HỌC VIÊN
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var (success, error, newStatus) = await _hocVienService.DoiTrangThaiAsync(id);
            return Json(new { success, message = error, newStatus });
        }

        // ========================================================
        // 9. AJAX XÓA HỌC VIÊN
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Json(new { success = false, message = "Bạn không có quyền quản trị." });
            }

            var (success, error) = await _hocVienService.XoaHocVienAsync(id);
            return Json(new { success, message = error ?? "Đã xóa học viên thành công!" });
        }

        // ========================================================
        // ACTION DELETE MULTIPLE: XÓA NHIỀU HỌC VIÊN
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int>? ids)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Json(new { success = false, message = "Bạn không có quyền quản trị." });
            }

            if (ids == null || !ids.Any())
                return Json(new { success = false, message = "Vui lòng chọn ít nhất 1 học viên để xóa." });

            int deleted = 0;
            int skipped = 0;

            foreach (var id in ids)
            {
                var (success, _) = await _hocVienService.XoaHocVienAsync(id);
                if (success) deleted++;
                else skipped++;
            }

            if (deleted == 0 && skipped > 0)
            {
                return Json(new { success = false, message = $"Không thể xóa {skipped} học viên đã chọn do đang có lịch sử đăng ký học hoặc kết quả đào tạo." });
            }

            string msg = $"Đã xóa thành công {deleted} học viên.";
            if (skipped > 0)
            {
                msg += $" Bỏ qua {skipped} học viên đang có dữ liệu đăng ký/kết quả học tập liên quan.";
            }
            return Json(new { success = true, message = msg, deletedCount = deleted, skippedCount = skipped });
        }

        [HttpGet]
        public async Task<IActionResult> GetHocVienJson(int id)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Json(new { success = false, message = "Bạn không có quyền quản trị." });
            }

            var hv = await _hocVienRepo.GetByIdWithDetailsAsync(id);

            if (hv == null) return Json(new { success = false, message = "Không tìm thấy học viên." });

            return Json(new
            {
                success = true,
                data = new
                {
                    maHocVien = hv.MaHocVien,
                    hoTen = hv.HoTen,
                    email = hv.Email,
                    soDienThoai = hv.SoDienThoai ?? "",
                    ngaySinh = hv.NgaySinh?.ToString("yyyy-MM-dd") ?? "",
                    gioiTinh = hv.GioiTinh ?? "Nam",
                    diaChi = hv.DiaChi ?? "",
                    trinhDoHocVan = hv.TrinhDoHocVan ?? "Đại học",
                    ngheNghiep = hv.NgheNghiep ?? "",
                    trangThai = hv.TrangThai,
                    tenDangNhap = hv.TaiKhoan?.TenDangNhap ?? "",
                    soLop = hv.DangKyHocs.Count
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(HocVien hocVien)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                if (isAjax) return Json(new { success = false, message = "Bạn không có quyền quản trị." });
                return redirectResult!;
            }

            if (!hocVien.NgaySinh.HasValue)
            {
                ModelState.AddModelError(nameof(HocVien.NgaySinh), "Ngày tháng năm sinh không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(hocVien.SoDienThoai) || !System.Text.RegularExpressions.Regex.IsMatch(hocVien.SoDienThoai.Trim(), @"^0\d{9}$"))
            {
                ModelState.AddModelError(nameof(HocVien.SoDienThoai), "Số điện thoại phải gồm đúng 10 chữ số và bắt đầu bằng số 0 (Ví dụ: 0912345678).");
            }

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = errors });
                }
                return View(nameof(Create), hocVien);
            }

            var (success, error) = await _hocVienService.ThemHocVienAsync(hocVien);
            if (!success)
            {
                if (isAjax) return Json(new { success = false, message = error });
                ModelState.AddModelError("", error!);
                return View(nameof(Create), hocVien);
            }

            string msg = $"Thêm mới học viên '{hocVien.HoTen}' thành công!";
            if (isAjax) return Json(new { success = true, message = msg });
            TempData["Success"] = msg;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, HocVien hocVien)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                if (isAjax) return Json(new { success = false, message = "Bạn không có quyền quản trị." });
                return redirectResult!;
            }

            if (id != hocVien.MaHocVien) return NotFound();

            if (!hocVien.NgaySinh.HasValue)
            {
                ModelState.AddModelError(nameof(HocVien.NgaySinh), "Ngày tháng năm sinh không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(hocVien.SoDienThoai) || !System.Text.RegularExpressions.Regex.IsMatch(hocVien.SoDienThoai.Trim(), @"^0\d{9}$"))
            {
                ModelState.AddModelError(nameof(HocVien.SoDienThoai), "Số điện thoại phải gồm đúng 10 chữ số và bắt đầu bằng số 0 (Ví dụ: 0912345678).");
            }

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = errors });
                }
                return View(nameof(Edit), hocVien);
            }

            var (success, error) = await _hocVienService.CapNhatHocVienAsync(hocVien);
            if (!success)
            {
                if (isAjax) return Json(new { success = false, message = error });
                ModelState.AddModelError("", error!);
                return View(nameof(Edit), hocVien);
            }

            string msg = $"Cập nhật thông tin học viên '{hocVien.HoTen}' thành công!";
            if (isAjax) return Json(new { success = true, message = msg });
            TempData["Success"] = msg;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                if (isAjax) return Json(new { success = false, message = "Bạn không có quyền quản trị." });
                return redirectResult!;
            }

            var (success, error) = await _hocVienService.XoaHocVienAsync(id);
            if (!success)
            {
                if (isAjax) return Json(new { success = false, message = error });
                TempData["Error"] = error;
            }
            else
            {
                if (isAjax) return Json(new { success = true, message = "Đã xóa học viên thành công!" });
                TempData["Success"] = "Đã xóa học viên thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 10. ACTION DETAILS: XEM CHI TIẾT ĐẦY ĐỦ
        // ========================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            if (id == null) return NotFound();

            var hocVien = await _hocVienService.GetChiTietHocVienAsync(id);
            if (hocVien == null) return NotFound();

            return View(hocVien);
        }

        // ========================================================
        // 11. ACTION CREATE: THÊM MỚI HỌC VIÊN
        // ========================================================
        public IActionResult Create()
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            return RedirectToAction(nameof(Index), new { create = "true" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HocVien hocVien)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            if (!hocVien.NgaySinh.HasValue)
            {
                ModelState.AddModelError(nameof(HocVien.NgaySinh), "Ngày tháng năm sinh không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(hocVien.SoDienThoai) || !System.Text.RegularExpressions.Regex.IsMatch(hocVien.SoDienThoai.Trim(), @"^0\d{9}$"))
            {
                ModelState.AddModelError(nameof(HocVien.SoDienThoai), "Số điện thoại phải gồm đúng 10 chữ số và bắt đầu bằng số 0 (Ví dụ: 0912345678).");
            }

            if (!ModelState.IsValid)
            {
                return View(hocVien);
            }

            var (success, error) = await _hocVienService.ThemHocVienAsync(hocVien);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(hocVien);
            }

            TempData["Success"] = $"Thêm mới học viên '{hocVien.HoTen}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 12. ACTION EDIT: CHỈNH SỬA HỌC VIÊN
        // ========================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            if (id == null) return NotFound();

            var hocVien = await _hocVienService.GetHocVienByIdAsync(id);
            if (hocVien == null) return NotFound();

            return View(hocVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HocVien hocVien)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            if (id != hocVien.MaHocVien) return NotFound();

            if (!hocVien.NgaySinh.HasValue)
            {
                ModelState.AddModelError(nameof(HocVien.NgaySinh), "Ngày tháng năm sinh không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(hocVien.SoDienThoai) || !System.Text.RegularExpressions.Regex.IsMatch(hocVien.SoDienThoai.Trim(), @"^0\d{9}$"))
            {
                ModelState.AddModelError(nameof(HocVien.SoDienThoai), "Số điện thoại phải gồm đúng 10 chữ số và bắt đầu bằng số 0 (Ví dụ: 0912345678).");
            }

            if (!ModelState.IsValid)
            {
                return View(hocVien);
            }

            var (success, error) = await _hocVienService.CapNhatHocVienAsync(hocVien);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(hocVien);
            }

            TempData["Success"] = $"Cập nhật thông tin học viên '{hocVien.HoTen}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 13. ACTION DELETE: XÓA HỌC VIÊN TRUYỀN THỐNG
        // ========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            var (success, error) = await _hocVienService.XoaHocVienAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
            }
            else
            {
                TempData["Success"] = "Đã xóa học viên thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 14. ACTION PORTAL: CỔNG THÔNG TIN HỌC VIÊN (Yêu cầu đăng nhập)
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Portal()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan", new { returnUrl = "/HocVien/Portal" });
            }

            var hocVien = await _hocVienRepo.GetByMaTaiKhoanAsync(maTaiKhoan.Value);

            if (hocVien == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ học viên liên kết với tài khoản này.";
                return RedirectToAction("Index", "Home");
            }

            var allLops = await _lopHocRepo.GetAllAsync();
            var lopHocDangMos = allLops.Where(l => l.TrangThai == "Đang mở đăng ký").ToList();

            var allCtdt = await _ctdtRepo.GetAllAsync();
            var chuongTrinhs = allCtdt.Where(c => c.TrangThai).ToList();

            var viewModel = new CongHocVienViewModel
            {
                HocVien = hocVien,
                LopHocDangMos = lopHocDangMos,
                DanhSachDangKy = hocVien.DangKyHocs.ToList(),
                ChuongTrinhs = chuongTrinhs
            };

            return View(viewModel);
        }
    }
}
