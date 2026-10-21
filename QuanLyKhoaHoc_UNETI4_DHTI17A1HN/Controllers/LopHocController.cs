using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học, Tìm kiếm, Lọc, Sắp xếp, Phân trang LINQ
    // Controller chuẩn kiến trúc: KHÔNG truy vấn DbContext, chỉ tiêm ILopHocService
    // ========================================================
    public class LopHocController : Controller
    {
        private readonly ILopHocService _lopHocService;

        public LopHocController(ILopHocService lopHocService)
        {
            _lopHocService = lopHocService;
        }

        // ========================================================
        // 1. GET: /LopHoc/Index
        // Hiển thị danh sách, tích hợp Tìm kiếm LINQ, Lọc đa điều kiện, Sắp xếp & Phân trang server
        // ========================================================
        public async Task<IActionResult> Index(
            string? keyword,
            int? maChuongTrinh,
            string? trinhDo,
            string? trangThai,
            int? maGiangVien,
            decimal? minHocPhi,
            decimal? maxHocPhi,
            string? filterHanDangKy,
            string? filterConCho,
            string? sortBy,
            int page = 1)
        {
            var viewModel = await _lopHocService.GetDanhSachLopAsync(
                keyword,
                maChuongTrinh,
                trinhDo,
                trangThai,
                maGiangVien,
                minHocPhi,
                maxHocPhi,
                filterHanDangKy,
                filterConCho,
                sortBy,
                page,
                pageSize: 8);

            return View(viewModel);
        }

        // ========================================================
        // 2. GET: /LopHoc/Details/5
        // Xem chi tiết lớp học kèm tiến độ sĩ số & danh sách đăng ký
        // ========================================================
        public async Task<IActionResult> Details(int id)
        {
            var lopHoc = await _lopHocService.GetChiTietLopAsync(id);
            if (lopHoc == null)
            {
                TempData["Error"] = "Không tìm thấy lớp học yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SoHocVienDaDuyet = await _lopHocService.GetSoLuongHocVienDaDuyetAsync(id);
            return View(lopHoc);
        }

        // ========================================================
        // 3. GET: /LopHoc/Create
        // Mở giao diện tạo lớp học mới
        // ========================================================
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();

            var lopHoc = new LopHoc
            {
                NgayBatDauDangKy = DateTime.Today,
                HanDangKy = DateTime.Today.AddDays(15),
                NgayKhaiGiang = DateTime.Today.AddDays(20),
                SiSoToiDa = 30,
                SoBuoiHoc = 12,
                TrangThai = "Đang mở đăng ký",
                TrinhDoDauVao = "Cơ bản"
            };

            return View(lopHoc);
        }

        // ========================================================
        // 4. POST: /LopHoc/Create
        // Tiếp nhận và tạo mới lớp học qua Service Layer
        // ========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LopHoc lopHoc)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(lopHoc.MaChuongTrinh, lopHoc.MaGiangVien);
                return View(lopHoc);
            }

            var (success, error) = await _lopHocService.TaoLopHocAsync(lopHoc);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                await PopulateDropdownsAsync(lopHoc.MaChuongTrinh, lopHoc.MaGiangVien);
                return View(lopHoc);
            }

            TempData["Success"] = $"Mở lớp học \"{lopHoc.TenLop}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 5. GET: /LopHoc/Edit/5
        // Mở giao diện chỉnh sửa lớp học
        // ========================================================
        public async Task<IActionResult> Edit(int id)
        {
            var lopHoc = await _lopHocService.GetChiTietLopAsync(id);
            if (lopHoc == null)
            {
                TempData["Error"] = "Không tìm thấy lớp học cần chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync(lopHoc.MaChuongTrinh, lopHoc.MaGiangVien);
            ViewBag.SoHocVienDaDuyet = await _lopHocService.GetSoLuongHocVienDaDuyetAsync(id);
            return View(lopHoc);
        }

        // ========================================================
        // 6. POST: /LopHoc/Edit/5
        // Cập nhật thông tin lớp học qua Service Layer
        // ========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LopHoc lopHoc)
        {
            if (id != lopHoc.MaLopHoc)
            {
                TempData["Error"] = "Mã lớp học không trùng khớp.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(lopHoc.MaChuongTrinh, lopHoc.MaGiangVien);
                ViewBag.SoHocVienDaDuyet = await _lopHocService.GetSoLuongHocVienDaDuyetAsync(id);
                return View(lopHoc);
            }

            var (success, error) = await _lopHocService.CapNhatLopHocAsync(lopHoc);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                await PopulateDropdownsAsync(lopHoc.MaChuongTrinh, lopHoc.MaGiangVien);
                ViewBag.SoHocVienDaDuyet = await _lopHocService.GetSoLuongHocVienDaDuyetAsync(id);
                return View(lopHoc);
            }

            TempData["Success"] = $"Cập nhật lớp học \"{lopHoc.TenLop}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // 7. POST: /LopHoc/Delete/5
        // Xóa lớp học (Kiểm soát chặt chẽ không cho xóa nếu có học viên đăng ký)
        // ========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _lopHocService.XoaLopHocAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
            }
            else
            {
                TempData["Success"] = "Đã xóa lớp học thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // Helper: Nạp dữ liệu danh sách chọn cho View
        // ========================================================
        private async Task PopulateDropdownsAsync(int? selectedChuongTrinh = null, int? selectedGiangVien = null)
        {
            var chuongTrinhs = await _lopHocService.GetDanhSachChuongTrinhAsync();
            var giangViens = await _lopHocService.GetDanhSachGiangVienAsync();

            ViewBag.MaChuongTrinh = new SelectList(chuongTrinhs, "MaChuongTrinh", "TenChuongTrinh", selectedChuongTrinh);
            ViewBag.MaGiangVien = new SelectList(giangViens, "MaGiangVien", "HoTen", selectedGiangVien);
        }
    }
}
