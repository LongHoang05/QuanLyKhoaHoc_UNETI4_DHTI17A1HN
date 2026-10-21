// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Quản lý Dữ liệu dùng chung (Giảng viên)
// Luồng xử lý: Controller -> Service -> Repository -> ApplicationDbContext
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    [SessionAuthorize("Admin", "GiaoVu")]
    public class GiangVienController : Controller
    {
        private readonly IGiangVienService _giangVienService;

        public GiangVienController(IGiangVienService giangVienService)
        {
            _giangVienService = giangVienService;
        }

        // ========================================================
        // 1. DANH SÁCH GIẢNG VIÊN (Tìm kiếm + Lọc + Phân trang LINQ)
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, bool? trangThai, int page = 1)
        {
            var (items, totalCount, totalPages, pageIndex) = await _giangVienService.GetPagedAsync(
                keyword, trangThai, page, 8);

            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageIndex = pageIndex;

            return View(items);
        }

        // ========================================================
        // 2. CHI TIẾT GIẢNG VIÊN
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var gv = await _giangVienService.GetByIdWithLopHocsAsync(id);
            if (gv == null)
            {
                return NotFound();
            }
            return View(gv);
        }

        // ========================================================
        // 3. THÊM MỚI GIẢNG VIÊN
        // ========================================================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new GiangVien { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiangVien giangVien)
        {
            if (!ModelState.IsValid)
            {
                return View(giangVien);
            }

            var (success, errorMessage) = await _giangVienService.ThemGiangVienAsync(giangVien);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Không thể thêm giảng viên.");
                return View(giangVien);
            }

            TempData["Success"] = $"Thêm giảng viên '{giangVien.HoTen}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 4. CHỈNH SỬA GIẢNG VIÊN
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var gv = await _giangVienService.GetByIdAsync(id);
            if (gv == null)
            {
                return NotFound();
            }
            return View(gv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GiangVien giangVien)
        {
            if (id != giangVien.MaGiangVien)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(giangVien);
            }

            var (success, errorMessage) = await _giangVienService.CapNhatGiangVienAsync(giangVien);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Không thể cập nhật giảng viên.");
                return View(giangVien);
            }

            TempData["Success"] = $"Cập nhật giảng viên '{giangVien.HoTen}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 5. XÓA GIẢNG VIÊN (Kiểm soát an toàn: Chặn nếu có lớp học phụ trách)
        // ========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await _giangVienService.XoaGiangVienAsync(id);
            if (!success)
            {
                TempData["Error"] = errorMessage;
            }
            else
            {
                TempData["Success"] = "Đã xóa giảng viên thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 6. ĐỔI TRẠNG THÁI GIẢNG VIÊN
        // ========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errorMessage) = await _giangVienService.DoiTrangThaiAsync(id);
            if (!success)
            {
                TempData["Error"] = errorMessage;
            }
            else
            {
                TempData["Success"] = "Đã cập nhật trạng thái giảng viên!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
