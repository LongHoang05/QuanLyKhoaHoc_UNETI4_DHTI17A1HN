using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    /// <summary>
    /// Controller Trang chủ - Tổng quan Hệ thống Quản lý Đào tạo & Khóa học
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Thống kê số liệu hệ thống
                var totalChuongTrinh = await _context.ChuongTrinhDaoTaos.CountAsync();
                var totalLopHoc = await _context.LopHocs.CountAsync();
                var openLopHoc = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký");
                var totalHocVien = await _context.HocViens.CountAsync();
                var totalTaiKhoan = await _context.TaiKhoans.CountAsync();
                var totalDangKy = await _context.DangKyHocs.CountAsync();
                var pendingDangKy = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ duyệt");
                var totalDoanhThu = await _context.DangKyHocs.SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0;

                ViewBag.TotalChuongTrinh = totalChuongTrinh;
                ViewBag.TotalLopHoc = totalLopHoc;
                ViewBag.OpenLopHoc = openLopHoc;
                ViewBag.TotalHocVien = totalHocVien;
                ViewBag.TotalTaiKhoan = totalTaiKhoan;
                ViewBag.TotalDangKy = totalDangKy;
                ViewBag.PendingDangKy = pendingDangKy;
                ViewBag.TotalDoanhThu = totalDoanhThu;

                // Danh sách chương trình đào tạo
                var chuongTrinhs = await _context.ChuongTrinhDaoTaos
                    .Include(c => c.LopHocs)
                    .OrderByDescending(c => c.MaChuongTrinh)
                    .Take(6)
                    .ToListAsync();

                // Danh sách lớp học mới nhất
                var lopHocs = await _context.LopHocs
                    .Include(l => l.ChuongTrinhDaoTao)
                    .OrderByDescending(l => l.MaLopHoc)
                    .Take(5)
                    .ToListAsync();

                // Danh sách đơn đăng ký gần đây
                var recentDangKys = await _context.DangKyHocs
                    .Include(d => d.HocVien)
                    .Include(d => d.LopHoc)
                    .OrderByDescending(d => d.MaDangKy)
                    .Take(5)
                    .ToListAsync();

                ViewBag.ChuongTrinhs = chuongTrinhs;
                ViewBag.LopHocs = lopHocs;
                ViewBag.RecentDangKys = recentDangKys;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải số liệu tổng quan hệ thống");
                ViewBag.TotalChuongTrinh = 0;
                ViewBag.TotalLopHoc = 0;
                ViewBag.OpenLopHoc = 0;
                ViewBag.TotalHocVien = 0;
                ViewBag.TotalTaiKhoan = 0;
                ViewBag.TotalDangKy = 0;
                ViewBag.PendingDangKy = 0;
                ViewBag.TotalDoanhThu = 0m;
                ViewBag.ChuongTrinhs = new List<ChuongTrinhDaoTao>();
                ViewBag.LopHocs = new List<LopHoc>();
                ViewBag.RecentDangKys = new List<DangKyHoc>();
            }

            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
