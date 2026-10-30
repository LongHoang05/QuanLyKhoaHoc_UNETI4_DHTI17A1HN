using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh
    // Mã sinh viên: 23103100042
    // Phụ trách: Module 3 - Quản lý Học viên (Mục 7.1 Tài liệu đặc tả)
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

        // GET: HocVien
        public async Task<IActionResult> Index(string? searchString, bool? trangThai, int? maLopHoc)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Hiển thị danh sách học viên, tìm kiếm theo từ khóa, lọc trạng thái và lớp học
            var danhSach = await _hocVienService.TimKiemHocVienAsync(searchString, trangThai, maLopHoc);
            return View(danhSach);
        }

        // GET: HocVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Xem chi tiết hồ sơ học viên và lịch sử đăng ký lớp học
            await Task.CompletedTask;
            return View(new HocVien { HoTen = "Học viên mẫu (TODO - Module 3)" });
        }

        // GET: HocVien/Create
        public IActionResult Create()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Hiển thị form thêm mới học viên
            return View(new HocVien());
        }

        // POST: HocVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HocVien hocVien)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Kiểm tra ModelState, gọi _hocVienService.ThemHocVienAsync(hocVien)
            await Task.CompletedTask;
            TempData["Error"] = "Chức năng Thêm học viên (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.";
            return RedirectToAction(nameof(Index));
        }

        // GET: HocVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Lấy thông tin học viên theo id và hiển thị form chỉnh sửa
            await Task.CompletedTask;
            return View(new HocVien { HoTen = "Học viên mẫu (TODO - Module 3)" });
        }

        // POST: HocVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HocVien hocVien)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Kiểm tra dữ liệu hợp lệ và gọi _hocVienService.CapNhatHocVienAsync(hocVien)
            await Task.CompletedTask;
            TempData["Error"] = "Chức năng Cập nhật học viên (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.";
            return RedirectToAction(nameof(Index));
        }

        // GET: HocVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Hiển thị xác nhận xóa/khóa học viên
            await Task.CompletedTask;
            return View(new HocVien { HoTen = "Học viên mẫu (TODO - Module 3)" });
        }

        // POST: HocVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Xử lý xóa hoặc khóa tài khoản học viên qua _hocVienService.XoaHocVienAsync(id)
            await Task.CompletedTask;
            TempData["Error"] = "Chức năng Xóa học viên (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.";
            return RedirectToAction(nameof(Index));
        }
    }
}
