// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tầng Service Quản lý Tài khoản & Phân quyền
// Nghiệp vụ doanh nghiệp: Đăng nhập, Đăng ký, Đổi mật khẩu, Khóa/Mở tài khoản
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    public interface ITaiKhoanService
    {
        /// <summary>
        /// Nghiệp vụ kiểm tra đăng nhập bằng EF Core & LINQ:
        /// Kiểm tra tài khoản tồn tại, mật khẩu chính xác, tài khoản không bị khóa.
        /// </summary>
        Task<(bool Success, string? ErrorMessage, TaiKhoan? User)> KiemTraDangNhapAsync(string tenDangNhap, string matKhau);

        /// <summary>
        /// Nghiệp vụ đăng ký tài khoản cho học viên:
        /// Kiểm tra tên đăng nhập và email duy nhất, tạo tài khoản và tự động khởi tạo hồ sơ học viên liên kết.
        /// </summary>
        Task<(bool Success, string? ErrorMessage, TaiKhoan? User)> DangKyHocVienAsync(DangKyViewModel model);

        /// <summary>
        /// Admin tạo tài khoản mới (Admin, Giáo vụ, Học viên)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> TaoTaiKhoanAsync(TaiKhoan taiKhoan);

        /// <summary>
        /// Admin cập nhật tài khoản
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> CapNhatTaiKhoanAsync(TaiKhoan taiKhoan);

        /// <summary>
        /// Đổi mật khẩu tài khoản
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DoiMatKhauAsync(int maTaiKhoan, string matKhauCu, string matKhauMoi);

        /// <summary>
        /// Bật/Tắt trạng thái hoạt động (Khóa hoặc Mở khóa tài khoản)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> KhoaMoKhoaTaiKhoanAsync(int maTaiKhoan);

        /// <summary>
        /// Đặt lại mật khẩu về mặc định cho tài khoản
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> ResetMatKhauAsync(int maTaiKhoan, string matKhauMoi = "123456");

        /// <summary>
        /// Lấy tài khoản theo ID
        /// </summary>
        Task<TaiKhoan?> GetTaiKhoanByIdAsync(int id);

        /// <summary>
        /// Lấy danh sách tài khoản có phân trang Skip/Take, tìm kiếm và lọc
        /// </summary>
        Task<TaiKhoanListViewModel> GetDanhSachTaiKhoanPagedAsync(
            string? keyword, string? vaiTro, bool? trangThai, int pageIndex, int pageSize);

        /// <summary>
        /// Lấy thông tin hồ sơ cá nhân của tài khoản đăng nhập
        /// </summary>
        Task<HoSoCaNhanViewModel?> LayHoSoCaNhanAsync(int maTaiKhoan);

        /// <summary>
        /// Cập nhật thông tin hồ sơ cá nhân
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> CapNhatHoSoCaNhanAsync(HoSoCaNhanViewModel model);
    }
}
