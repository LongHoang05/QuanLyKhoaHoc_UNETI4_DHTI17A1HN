// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Trang Quản Trị Hệ Thống (Admin Dashboard)
// Luồng xử lý: Controller -> Service -> Repository -> ApplicationDbContext
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    [SessionAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IChuongTrinhDaoTaoService _chuongTrinhService;
        private readonly IGiangVienService _giangVienService;

        public AdminController(
            ITaiKhoanService taiKhoanService,
            IChuongTrinhDaoTaoService chuongTrinhService,
            IGiangVienService giangVienService)
        {
            _taiKhoanService = taiKhoanService;
            _chuongTrinhService = chuongTrinhService;
            _giangVienService = giangVienService;
        }

        // ========================================================
        // TRANG QUẢN TRỊ ADMIN (Dashboard tổng quan toàn hệ thống)
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pagedAccounts = await _taiKhoanService.GetDanhSachTaiKhoanPagedAsync(null, null, null, 1, 100);
            var allAccounts = pagedAccounts.Items.ToList();

            var chuongTrinhs = await _chuongTrinhService.GetDanhSachAsync();
            var giangViens = await _giangVienService.GetDanhSachAsync();

            ViewBag.TotalTaiKhoan = pagedAccounts.TotalCount;
            ViewBag.CountAdmin = allAccounts.Count(a => a.VaiTro == "Admin");
            ViewBag.CountGiaoVu = allAccounts.Count(a => a.VaiTro == "GiaoVu");
            ViewBag.CountHocVien = allAccounts.Count(a => a.VaiTro == "HocVien");
            ViewBag.CountLocked = allAccounts.Count(a => !a.TrangThai);

            ViewBag.TotalChuongTrinh = chuongTrinhs.Count;
            ViewBag.TotalGiangVien = giangViens.Count;

            ViewBag.RecentAccounts = allAccounts.Take(5).ToList();
            ViewBag.RecentChuongTrinhs = chuongTrinhs.Take(5).ToList();

            // Dữ liệu phục vụ Biểu đồ Chart.js (Mục 19 Đề tài)
            ViewBag.ChuongTrinhNames = chuongTrinhs.Select(c => c.TenChuongTrinh).ToList();
            ViewBag.LopHocCounts = chuongTrinhs.Select(c => c.LopHocs?.Count ?? 0).ToList();

            return View();
        }
    }
}
