// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Quản lý Chương trình đào tạo (ChuongTrinhDaoTao)
// Luồng xử lý: Controller -> Service -> Repository -> ApplicationDbContext
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    public class ChuongTrinhDaoTaoController : Controller
    {
        private readonly IChuongTrinhDaoTaoService _chuongTrinhService;

        public ChuongTrinhDaoTaoController(IChuongTrinhDaoTaoService chuongTrinhService)
        {
            _chuongTrinhService = chuongTrinhService;
        }

        // ========================================================
        // 1. DANH SÁCH CHƯƠNG TRÌNH ĐÀO TẠO (Tìm kiếm + Lọc + Phân trang LINQ)
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, bool? trangThai, int page = 1)
        {
            var viewModel = await _chuongTrinhService.GetPagedAsync(keyword, trangThai, page, 8);
            return View(viewModel);
        }

        // ========================================================
        // 2. CHI TIẾT CHƯƠNG TRÌNH ĐÀO TẠO (Kèm danh sách các lớp học trực thuộc)
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var chuongTrinh = await _chuongTrinhService.GetByIdWithDetailsAsync(id);
            if (chuongTrinh == null)
            {
                return NotFound();
            }
            return View(chuongTrinh);
        }

        // ========================================================
        // 3. THÊM MỚI CHƯƠNG TRÌNH ĐÀO TẠO (Yêu cầu quyền Admin)
        // ========================================================
        [SessionAuthorize("Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ChuongTrinhDaoTao { TrangThai = true });
        }

        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChuongTrinhDaoTao chuongTrinh)
        {
            if (!ModelState.IsValid)
            {
                return View(chuongTrinh);
            }

            var (success, errorMessage) = await _chuongTrinhService.ThemChuongTrinhAsync(chuongTrinh);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Không thể thêm chương trình đào tạo.");
                return View(chuongTrinh);
            }

            TempData["Success"] = $"Thêm mới chương trình '{chuongTrinh.TenChuongTrinh}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 4. CHỈNH SỬA CHƯƠNG TRÌNH ĐÀO TẠO (Yêu cầu quyền Admin)
        // ========================================================
        [SessionAuthorize("Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var chuongTrinh = await _chuongTrinhService.GetByIdAsync(id);
            if (chuongTrinh == null)
            {
                return NotFound();
            }
            return View(chuongTrinh);
        }

        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChuongTrinhDaoTao chuongTrinh)
        {
            if (id != chuongTrinh.MaChuongTrinh)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(chuongTrinh);
            }

            var (success, errorMessage) = await _chuongTrinhService.CapNhatChuongTrinhAsync(chuongTrinh);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Không thể cập nhật chương trình đào tạo.");
                return View(chuongTrinh);
            }

            TempData["Success"] = $"Cập nhật chương trình '{chuongTrinh.TenChuongTrinh}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 5. XÓA CHƯƠNG TRÌNH ĐÀO TẠO (Yêu cầu quyền Admin)
        // Kiểm soát an toàn CSDL: Chặn xóa nếu có lớp học liên quan
        // ========================================================
        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await _chuongTrinhService.XoaChuongTrinhAsync(id);
            if (!success)
            {
                TempData["Error"] = errorMessage;
            }
            else
            {
                TempData["Success"] = "Đã xóa chương trình đào tạo thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 6. ĐỔI TRẠNG THÁI HOẠT ĐỘNG (Yêu cầu quyền Admin)
        // ========================================================
        [SessionAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errorMessage) = await _chuongTrinhService.ToggleTrangThaiAsync(id);
            if (!success)
            {
                TempData["Error"] = errorMessage;
            }
            else
            {
                TempData["Success"] = "Đã cập nhật trạng thái hoạt động của chương trình đào tạo!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
