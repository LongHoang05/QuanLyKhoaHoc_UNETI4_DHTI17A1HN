using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh - MSV: 23103100042
    // Module 3: HocVienController
    // Chuẩn kiến trúc phân lớp: Controller -> Service -> Repository -> ApplicationDbContext
    // Kết hợp kỹ thuật jQuery AJAX và Phân quyền Session theo giáo trình UNETI
    // ========================================================
    public class HocVienController : Controller
    {
        // Tiêm Service để xử lý quy tắc nghiệp vụ doanh nghiệp
        private readonly IHocVienService _hocVienService;

        public HocVienController(IHocVienService hocVienService)
        {
            _hocVienService = hocVienService;
        }

        #region Helper Phân Quyền Session (Theo Slide 8-11 Bài Giảng Cookie & Session)
        /// <summary>
        /// Xác thực vai trò người dùng trong Session: Chỉ Admin và GiaoVu mới có quyền quản lý toàn bộ học viên
        /// </summary>
        private bool CoQuyenQuanTri(out IActionResult? redirectResult)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");

            // 1. Chưa đăng nhập
            if (maTaiKhoan == null || string.IsNullOrEmpty(vaiTro))
            {
                TempData["Error"] = "Vui lòng đăng nhập với tài khoản Quản trị viên (Admin) hoặc Giáo vụ (GiaoVu) để truy cập chức năng Quản lý học viên.";
                redirectResult = RedirectToAction("Index", "Home");
                return false;
            }

            // 2. Đăng nhập nhưng vai trò là Học viên -> Chặn quyền truy cập
            if (vaiTro != "Admin" && vaiTro != "GiaoVu")
            {
                TempData["Error"] = "Tài khoản của bạn không có quyền truy cập chức năng Quản lý học viên. Chức năng này chỉ dành cho Admin và Giáo vụ.";
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
        #endregion

        // ========================================================
        // 1. ACTION INDEX: HIỂN THỊ DANH SÁCH (Controller -> Service -> Repository)
        // ========================================================
        public async Task<IActionResult> Index()
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            var danhSach = await _hocVienService.GetDanhSachHocVienAsync();
            return View(danhSach);
        }

        // ========================================================
        // 2. ACTION TÌM KIẾM AJAX (Slide 385, 387, 389 PPTX)
        // Nhận keyword từ jQuery $.ajax(), gọi Service -> Repo và trả PartialView
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> TimKiemAjax(string? keyword, string? searchType)
        {
            var danhSach = await _hocVienService.TimKiemHocVienAsync(keyword, searchType);
            return PartialView("_DanhSachHocVien", danhSach);
        }

        // ========================================================
        // 3. ACTION DETAILS: XEM CHI TIẾT
        // ========================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _hocVienService.GetChiTietHocVienAsync(id);
            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }

        // ========================================================
        // 4. ACTION CREATE: THÊM MỚI (Xử lý nghiệp vụ doanh nghiệp qua Service)
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

            // Gọi Service để thẩm định các quy tắc nghiệp vụ doanh nghiệp (Check trùng email, tuổi >= 6)
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
        // 5. ACTION EDIT: CHỈNH SỬA
        // ========================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _hocVienService.GetHocVienByIdAsync(id);
            if (hocVien == null)
            {
                return NotFound();
            }

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

            if (id != hocVien.MaHocVien)
            {
                return NotFound();
            }

            if (!hocVien.NgaySinh.HasValue)
            {
                ModelState.AddModelError(nameof(HocVien.NgaySinh), "Ngày tháng năm sinh không được để trống.");
            }

            if (!ModelState.IsValid)
            {
                return View(hocVien);
            }

            // Gọi Service để thẩm định nghiệp vụ (Kiểm tra trùng email người khác, kiểm tra ngày sinh)
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
        // 6. ACTION DELETE: XÓA (Xử lý ràng buộc khóa ngoại qua Service)
        // ========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            // Gọi Service để kiểm tra ràng buộc toàn vẹn CSDL
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
        // 7. ACTION TOGGLE STATUS: ĐỔI TRẠNG THÁI HOẠT ĐỘNG
        // Hỗ trợ cả AJAX (không load lại trang) và HTTP Request thông thường
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> ToggleStatusAjax(int id)
        {
            if (!CoQuyenQuanTri(out _))
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var (success, error) = await _hocVienService.DoiTrangThaiAsync(id);
            return Json(new { success, message = error ?? "Đã đổi trạng thái học viên thành công!" });
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!CoQuyenQuanTri(out var redirectResult))
            {
                return redirectResult!;
            }

            var (success, error) = await _hocVienService.DoiTrangThaiAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
            }
            else
            {
                TempData["Success"] = "Đã cập nhật trạng thái hoạt động của học viên!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
