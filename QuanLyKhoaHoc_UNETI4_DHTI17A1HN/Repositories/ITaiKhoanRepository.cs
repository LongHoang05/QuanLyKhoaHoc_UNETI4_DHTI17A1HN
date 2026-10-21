// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface ITaiKhoanRepository
    {
        Task<TaiKhoan?> GetByIdAsync(int id);
        Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap);
        Task<TaiKhoan?> GetByEmailAsync(string email);
        Task<List<TaiKhoan>> GetAllAsync();
        
        // Truy vấn LINQ: Tìm kiếm, lọc vai trò, lọc trạng thái, phân trang Skip & Take
        Task<(IEnumerable<TaiKhoan> Items, int TotalCount)> GetPagedAsync(
            string? keyword, string? vaiTro, bool? trangThai, int pageIndex, int pageSize);

        Task AddAsync(TaiKhoan taiKhoan);
        Task UpdateAsync(TaiKhoan taiKhoan);
        Task DeleteAsync(int id);
        Task<bool> ExistsByTenDangNhapAsync(string tenDangNhap, int? excludeId = null);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
        Task<bool> ExistsAsync(int id);
        Task SaveAsync();
    }
}
