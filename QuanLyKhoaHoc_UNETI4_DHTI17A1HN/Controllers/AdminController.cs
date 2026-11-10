// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
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

        public AdminController(
            ITaiKhoanService taiKhoanService,
            IChuongTrinhDaoTaoService chuongTrinhService)
        {
            _taiKhoanService = taiKhoanService;
            _chuongTrinhService = chuongTrinhService;
        }

        // ========================================================
        // TRANG QUẢN TRỊ ADMIN (Thống nhất chuyển hướng sang Dashboard tổng quan)
        // ========================================================
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "ThongKe");
        }
    }
}
