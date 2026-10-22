// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    public interface IChuongTrinhDaoTaoService
    {
        Task<List<ChuongTrinhDaoTao>> GetDanhSachAsync();
        Task<ChuongTrinhDaoTao?> GetByIdAsync(int id);
        Task<ChuongTrinhDaoTao?> GetByIdWithDetailsAsync(int id);
        Task<ChuongTrinhDaoTaoListViewModel> GetPagedAsync(string? keyword, bool? trangThai, int pageIndex, int pageSize = 8);
        Task<(bool Success, string? ErrorMessage)> ThemChuongTrinhAsync(ChuongTrinhDaoTao chuongTrinh);
        Task<(bool Success, string? ErrorMessage)> CapNhatChuongTrinhAsync(ChuongTrinhDaoTao chuongTrinh);
        Task<(bool Success, string? ErrorMessage)> XoaChuongTrinhAsync(int id);
        Task<(bool Success, string? ErrorMessage)> ToggleTrangThaiAsync(int id);
    }
}
