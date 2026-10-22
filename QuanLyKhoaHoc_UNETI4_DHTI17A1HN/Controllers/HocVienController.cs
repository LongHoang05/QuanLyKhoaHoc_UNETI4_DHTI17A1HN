using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh - MSV: 23103100042
    // Module 3: HocVienController
    // Quản lý 2 Tab chuẩn đề tài 16: TaiKhoan (Mục 5) & HocVien (Mục 7)
    // Chuẩn kiến trúc phân lớp: Controller -> Service -> Repository -> ApplicationDbContext
    // Kết hợp kỹ thuật jQuery AJAX và Phân quyền Session theo giáo trình UNETI
    // ========================================================
    public class HocVienController : Controller
    {
        private readonly IHocVienService _hocVienService;
        private readonly ApplicationDbContext _context;

        public HocVienController(IHocVienService hocVienService, ApplicationDbContext context)
        {
            _hocVienService = hocVienService;
            _context = context;
        }

        #region Helper Phân Quyền Session (Theo Slide 8-11 Bài Giảng Cookie & Session)
        /// <summary>
        /// Xác thực vai trò người dùng trong Session: Chỉ Admin và GiaoVu mới có quyền quản lý toàn bộ học viên
        /// </summary>
        private bool CoQuyenQuanTri(out IActionResult? redirectResult)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");

            // 1. Chưa đăng nhập -> Tự động khởi tạo phiên Giáo vụ mặc định để kiểm thử nhanh chóng
            if (maTaiKhoan == null || string.IsNullOrEmpty(vaiTro))
            {
                HttpContext.Session.SetInt32("MaTaiKhoan", 2);
                HttpContext.Session.SetString("TenDangNhap", "giaovu");
                HttpContext.Session.SetString("HoTen", "Giáo vụ Trung tâm");
                HttpContext.Session.SetString("VaiTro", "GiaoVu");
                vaiTro = "GiaoVu";
            }

            // 2. Đăng nhập nhưng vai trò là Học viên -> Chặn quyền truy cập (chuẩn giáo trình Phân quyền Session UNETI)
            if (vaiTro != "Admin" && vaiTro != "GiaoVu")
            {
                TempData["Error"] = "Tài khoản học viên không có quyền quản lý danh sách học viên của trung tâm! (Đã bảo vệ bằng Session)";
                redirectResult = RedirectToAction("Index", "Home");
                return false;
            }

            redirectResult = null;
            return true;
        }

        // Hỗ trợ thiết lập nhanh Session khi kiểm thử (Role = GiaoVu | Admin | HocVien)
        [HttpGet]
        public IActionResult DangNhapThuNghiem(string role = "GiaoVu")
        {
            if (role == "Admin")
            {
                HttpContext.Session.SetInt32("MaTaiKhoan", 1);
                HttpContext.Session.SetString("TenDangNhap", "admin");
                HttpContext.Session.SetString("HoTen", "Quản trị viên Hệ thống");
                HttpContext.Session.SetString("VaiTro", "Admin");
            }
            else if (role == "HocVien")
            {
                HttpContext.Session.SetInt32("MaTaiKhoan", 3);
                HttpContext.Session.SetString("TenDangNhap", "hocvien");
                HttpContext.Session.SetString("HoTen", "Học viên Nguyễn Văn An");
                HttpContext.Session.SetString("VaiTro", "HocVien");
            }
            else
            {
                HttpContext.Session.SetInt32("MaTaiKhoan", 2);
                HttpContext.Session.SetString("TenDangNhap", "giaovu");
                HttpContext.Session.SetString("HoTen", "Giáo vụ Trung tâm");
                HttpContext.Session.SetString("VaiTro", "GiaoVu");
            }

            TempData["Success"] = $"Đã thiết lập phiên đăng nhập thử nghiệm: {role}";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult DangXuatThuNghiem()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Đã đăng xuất và xóa toàn bộ dữ liệu Session.";
            return RedirectToAction("Index", "Home");
        }
        #endregion

        // ========================================================
        // 1. ACTION INDEX: HIỂN THỊ GIAO DIỆN CHÍNH 2 TAB (TaiKhoan & HocVien)
        // ========================================================
        public async Task<IActionResult> Index()
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            var accounts = await _hocVienService.GetDanhSachTaiKhoanAsync();
            var students = await _hocVienService.GetDanhSachHocVienAsync();
            var classes = await _hocVienService.GetDanhSachLopHocAsync();
            var (tongHocVien, soGiaoVu, soAdmin, soBiKhoa) = await _hocVienService.GetThongKeAsync();

            var viewModel = new HocVienIndexViewModel
            {
                DanhSachTaiKhoan = accounts,
                DanhSachHocVien = students,
                DanhSachLopHoc = classes,
                TongSoHocVien = tongHocVien,
                SoGiaoVu = soGiaoVu,
                SoAdmin = soAdmin,
                SoBiKhoa = soBiKhoa,
                CurrentTab = "accounts"
            };

            return View(viewModel);
        }

        // ========================================================
        // 2. AJAX FILTER HỌC VIÊN: TÌM KIẾM + TRẠNG THÁI + LỌC THEO LỚP HỌC
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> FilterHocVienAjax(string? keyword, string? status, int? maLopHoc)
        {
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
            bool? trangThai = null;
            if (status == "active") trangThai = true;
            else if (status == "locked") trangThai = false;

            string? vaiTro = null;
            if (role == "Admin") vaiTro = "Admin";
            else if (role == "Giáo vụ" || role == "GiaoVu") vaiTro = "GiaoVu";
            else if (role == "Học viên" || role == "HocVien") vaiTro = "HocVien";

            var danhSach = await _hocVienService.TimKiemTaiKhoanAsync(keyword, vaiTro, trangThai);
            return PartialView("_DanhSachTaiKhoan", danhSach);
        }

        // ========================================================
        // 4. AJAX LẤY CHI TIẾT HỌC VIÊN RENDER MODAL (Chuẩn admin-tai-khoan.html)
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> GetChiTietHocVienJson(int id)
        {
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

            return View(new HocVien { TrangThai = true });
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
        // 14. ACTION PORTAL: CỔNG THÔNG TIN HỌC VIÊN (Chuẩn Giao_DIen_Test hoc-vien.html)
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Portal()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            HocVien? hocVien = null;

            if (maTaiKhoan != null)
            {
                hocVien = await _context.HocViens
                    .Include(h => h.TaiKhoan)
                    .Include(h => h.DangKyHocs)
                        .ThenInclude(d => d.LopHoc)
                            .ThenInclude(l => l!.ChuongTrinhDaoTao)
                    .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);
            }

            if (hocVien == null)
            {
                hocVien = await _context.HocViens
                    .Include(h => h.TaiKhoan)
                    .Include(h => h.DangKyHocs)
                        .ThenInclude(d => d.LopHoc)
                            .ThenInclude(l => l!.ChuongTrinhDaoTao)
                    .FirstOrDefaultAsync() ?? new HocVien { HoTen = "Học viên Test", Email = "test@uneti.edu.vn" };
            }

            var lopHocDangMos = await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Where(l => l.TrangThai == "Đang mở đăng ký")
                .ToListAsync();

            var chuongTrinhs = await _context.ChuongTrinhDaoTaos.Where(c => c.TrangThai).ToListAsync();

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
