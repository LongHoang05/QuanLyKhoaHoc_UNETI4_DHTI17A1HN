// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ITaiKhoanService taiKhoanService, ApplicationDbContext context)
        {
            _taiKhoanService = taiKhoanService;
            _context = context;
        }

        // ========================================================
        // 1. ĐĂNG NHẬP (Hỗ trợ cả URL /TaiKhoan/DangNhap và /TaiKhoan/Login)
        // ========================================================
        [HttpGet]
        [Route("TaiKhoan/DangNhap")]
        [Route("TaiKhoan/Login")]
        public IActionResult DangNhap(string? returnUrl = null, bool switchAccount = false)
        {
            // Nếu đã đăng nhập rồi và không phải bấm chuyển tài khoản thì chuyển hướng
            if (!switchAccount && HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new DangNhapViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/DangNhap")]
        [Route("TaiKhoan/Login")]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage, user) = await _taiKhoanService.KiemTraDangNhapAsync(
                model.TenDangNhap, model.MatKhau);

            if (!success || user == null)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Đăng nhập không thành công.");
                return View(model);
            }

            // Xóa sạch Session cũ trước khi ghi nhận phiên đăng nhập mới
            HttpContext.Session.Clear();

            // Ghi nhận thông tin vào Session theo yêu cầu mục 5.2 của đề tài
            HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);
            HttpContext.Session.SetString("Email", user.Email);

            var gv = await _context.GiangViens.FirstOrDefaultAsync(g => g.MaTaiKhoan == user.MaTaiKhoan);
            if (gv != null)
            {
                HttpContext.Session.SetInt32("MaGiangVien", gv.MaGiangVien);
            }

            var hv = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == user.MaTaiKhoan);
            if (hv != null)
            {
                HttpContext.Session.SetInt32("MaHocVien", hv.MaHocVien);
            }

            TempData["Success"] = $"Chào mừng {user.HoTen} ({user.VaiTro}) đã đăng nhập thành công!";

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            // Điều hướng trang đích dựa trên vai trò
            return user.VaiTro switch
            {
                "Admin" => RedirectToAction(nameof(Index)),
                "GiaoVu" => RedirectToAction("Index", "ChuongTrinhDaoTao"),
                "GiangVien" => RedirectToAction("Index", "GiangVien"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // ========================================================
        // 2. ĐĂNG KÝ HỌC VIÊN (Hỗ trợ /TaiKhoan/DangKy và /TaiKhoan/Register)
        // ========================================================
        [HttpGet]
        [Route("TaiKhoan/DangKy")]
        [Route("TaiKhoan/Register")]
        public IActionResult DangKy()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new DangKyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/DangKy")]
        [Route("TaiKhoan/Register")]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage, user) = await _taiKhoanService.DangKyHocVienAsync(model);
            if (!success || user == null)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Đăng ký không thành công.");
                return View(model);
            }

            // Tự động đăng nhập luôn sau khi đăng ký thành công
            HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);
            HttpContext.Session.SetString("Email", user.Email);

            var hv = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == user.MaTaiKhoan);
            if (hv != null)
            {
                HttpContext.Session.SetInt32("MaHocVien", hv.MaHocVien);
            }

            TempData["Success"] = "Đăng ký tài khoản học viên thành công! Bạn đã được đăng nhập tự động.";
            return RedirectToAction("Index", "Home");
        }

        // ========================================================
        // 3. ĐĂNG XUẤT (Xóa Session theo mục 5.3)
        // ========================================================
        [HttpGet, HttpPost]
        [Route("TaiKhoan/DangXuat")]
        [Route("TaiKhoan/Logout")]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Bạn đã đăng xuất khỏi hệ thống thành công.";
            return RedirectToAction(nameof(DangNhap));
        }

        // ========================================================
        // 4. HỒ SƠ CÁ NHÂN (Xem & Sửa thông tin tài khoản)
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction(nameof(DangNhap));
            }

            var hoSo = await _taiKhoanService.LayHoSoCaNhanAsync(maTaiKhoan.Value);
            if (hoSo == null)
            {
                return NotFound();
            }

            return View(hoSo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(HoSoCaNhanViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null || maTaiKhoan.Value != model.MaTaiKhoan)
            {
                return RedirectToAction(nameof(DangNhap));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage) = await _taiKhoanService.CapNhatHoSoCaNhanAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Cập nhật hồ sơ thất bại.");
                return View(model);
            }

            // Cập nhật lại session hiển thị
            HttpContext.Session.SetString("HoTen", model.HoTen);
            HttpContext.Session.SetString("Email", model.Email);

            TempData["Success"] = "Cập nhật thông tin hồ sơ cá nhân thành công!";
            return RedirectToAction(nameof(Profile));
        }

        // ========================================================
        // 5. ĐỔI MẬT KHẨU
        // ========================================================
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
            {
                return RedirectToAction(nameof(DangNhap));
            }
            return View(new DoiMatKhauViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction(nameof(DangNhap));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage) = await _taiKhoanService.DoiMatKhauAsync(
                maTaiKhoan.Value, model.MatKhauCu, model.MatKhauMoi);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Đổi mật khẩu thất bại.");
                return View(model);
            }

            TempData["Success"] = "Đổi mật khẩu thành công! Vui lòng ghi nhớ mật khẩu mới.";
            return RedirectToAction(nameof(Profile));
        }

        // ========================================================
        // 6. QUẢN LÝ DANH SÁCH TÀI KHOẢN (Chỉ dành cho ADMIN)
        // Tìm kiếm + Lọc vai trò + Lọc trạng thái + Phân trang Skip/Take
        // ========================================================
        [SessionAuthorize("Admin")]
        public async Task<IActionResult> Index(string? keyword, string? vaiTro, bool? trangThai, int page = 1)
        {
            var viewModel = await _taiKhoanService.GetDanhSachTaiKhoanPagedAsync(
                keyword, vaiTro, trangThai, page, 10);
            return View(viewModel);
        }

        // ========================================================
        // 7. TẠO TÀI KHOẢN MỚI (Admin)
        // ========================================================
        [SessionAuthorize("Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new TaiKhoan { TrangThai = true, VaiTro = "HocVien" });
        }

        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoan taiKhoan)
        {
            if (!ModelState.IsValid)
            {
                return View(taiKhoan);
            }

            var (success, errorMessage) = await _taiKhoanService.TaoTaiKhoanAsync(taiKhoan);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Không thể tạo tài khoản.");
                return View(taiKhoan);
            }

            TempData["Success"] = $"Tạo tài khoản '{taiKhoan.TenDangNhap}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 8. CHỈNH SỬA TÀI KHOẢN (Admin)
        // ========================================================
        [SessionAuthorize("Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var taiKhoan = await _taiKhoanService.GetTaiKhoanByIdAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            return View(taiKhoan);
        }

        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.MaTaiKhoan)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(taiKhoan);
            }

            var (success, errorMessage) = await _taiKhoanService.CapNhatTaiKhoanAsync(taiKhoan);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Cập nhật tài khoản thất bại.");
                return View(taiKhoan);
            }

            TempData["Success"] = $"Cập nhật tài khoản '{taiKhoan.TenDangNhap}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 9. KHÓA / MỞ KHÓA TÀI KHOẢN (Admin)
        // ========================================================
        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errorMessage) = await _taiKhoanService.KhoaMoKhoaTaiKhoanAsync(id);
            if (!success)
            {
                TempData["Error"] = errorMessage;
            }
            else
            {
                TempData["Success"] = "Đã cập nhật trạng thái tài khoản!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 10. ĐẶT LẠI MẬT KHẨU MẶC ĐỊNH (Admin)
        // ========================================================
        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var (success, errorMessage) = await _taiKhoanService.ResetMatKhauAsync(id, "123456");
            if (!success)
            {
                TempData["Error"] = errorMessage;
            }
            else
            {
                TempData["Success"] = "Đã đặt lại mật khẩu về mặc định '123456'!";
            }
            return RedirectToAction(nameof(Index));
        }

        [SessionAuthorize("Admin")]
        [HttpGet]
        public async Task<IActionResult> GetTaiKhoanJson(int id)
        {
            var tk = await _taiKhoanService.GetTaiKhoanByIdAsync(id);
            if (tk == null) return Json(new { success = false, message = "Không tìm thấy tài khoản." });
            return Json(new
            {
                success = true,
                data = new
                {
                    maTaiKhoan = tk.MaTaiKhoan,
                    tenDangNhap = tk.TenDangNhap,
                    hoTen = tk.HoTen,
                    email = tk.Email,
                    vaiTro = tk.VaiTro,
                    trangThai = tk.TrangThai
                }
            });
        }

        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(string tenDangNhap, string hoTen, string email, string vaiTro, bool trangThai, string? matKhau)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var tk = new TaiKhoan
            {
                TenDangNhap = tenDangNhap?.Trim() ?? "",
                HoTen = hoTen?.Trim() ?? "",
                Email = email?.Trim() ?? "",
                VaiTro = string.IsNullOrWhiteSpace(vaiTro) ? "HocVien" : vaiTro,
                TrangThai = trangThai,
                MatKhau = string.IsNullOrWhiteSpace(matKhau) ? "123456" : matKhau
            };

            var (success, errorMessage) = await _taiKhoanService.TaoTaiKhoanAsync(tk);
            if (!success)
            {
                if (isAjax) return Json(new { success = false, message = errorMessage ?? "Không thể tạo tài khoản." });
                TempData["Error"] = errorMessage;
                return RedirectToAction(nameof(Index));
            }

            string msg = $"Tạo tài khoản '{tk.TenDangNhap}' thành công!";
            if (isAjax) return Json(new { success = true, message = msg });
            TempData["Success"] = msg;
            return RedirectToAction(nameof(Index));
        }

        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int maTaiKhoan, string tenDangNhap, string hoTen, string email, string vaiTro, bool trangThai)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var tk = await _taiKhoanService.GetTaiKhoanByIdAsync(maTaiKhoan);
            if (tk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy tài khoản." });
                TempData["Error"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            tk.TenDangNhap = tenDangNhap?.Trim() ?? tk.TenDangNhap;
            tk.HoTen = hoTen?.Trim() ?? tk.HoTen;
            tk.Email = email?.Trim() ?? tk.Email;
            tk.VaiTro = string.IsNullOrWhiteSpace(vaiTro) ? tk.VaiTro : vaiTro;
            tk.TrangThai = trangThai;

            var (success, errorMessage) = await _taiKhoanService.CapNhatTaiKhoanAsync(tk);
            if (!success)
            {
                if (isAjax) return Json(new { success = false, message = errorMessage ?? "Cập nhật tài khoản thất bại." });
                TempData["Error"] = errorMessage;
                return RedirectToAction(nameof(Index));
            }

            string msg = $"Cập nhật tài khoản '{tk.TenDangNhap}' thành công!";
            if (isAjax) return Json(new { success = true, message = msg });
            TempData["Success"] = msg;
            return RedirectToAction(nameof(Index));
        }

        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatusAjax(int id)
        {
            var (success, errorMessage) = await _taiKhoanService.KhoaMoKhoaTaiKhoanAsync(id);
            var tk = await _taiKhoanService.GetTaiKhoanByIdAsync(id);
            return Json(new { success, message = errorMessage ?? "Cập nhật trạng thái thành công!", newStatus = tk?.TrangThai ?? false });
        }

        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordPost(int id, string? newPassword)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            string pwd = string.IsNullOrWhiteSpace(newPassword) ? "123456" : newPassword;
            var (success, errorMessage) = await _taiKhoanService.ResetMatKhauAsync(id, pwd);
            if (!success)
            {
                if (isAjax) return Json(new { success = false, message = errorMessage });
                TempData["Error"] = errorMessage;
            }
            else
            {
                if (isAjax) return Json(new { success = true, message = $"Đã đặt lại mật khẩu về '{pwd}' thành công!" });
                TempData["Success"] = $"Đã đặt lại mật khẩu về '{pwd}' thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var tk = await _context.TaiKhoans.FindAsync(id);
            if (tk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy tài khoản." });
                TempData["Error"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            if (tk.VaiTro == "Admin" && tk.TenDangNhap.ToLower() == "admin")
            {
                string warn = "Không thể xóa tài khoản Quản trị viên cấp cao nhất!";
                if (isAjax) return Json(new { success = false, message = warn });
                TempData["Error"] = warn;
                return RedirectToAction(nameof(Index));
            }

            _context.TaiKhoans.Remove(tk);
            await _context.SaveChangesAsync();

            string msg = $"Đã xóa tài khoản '{tk.TenDangNhap}' thành công!";
            if (isAjax) return Json(new { success = true, message = msg });
            TempData["Success"] = msg;
            return RedirectToAction(nameof(Index));
        }
    }
}
