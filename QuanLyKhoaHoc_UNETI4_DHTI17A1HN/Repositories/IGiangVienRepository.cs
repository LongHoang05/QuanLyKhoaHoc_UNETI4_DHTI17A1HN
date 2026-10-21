// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Quản lý Dữ liệu dùng chung (Giảng viên)
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface IGiangVienRepository
    {
        Task<List<GiangVien>> GetAllAsync();
        Task<GiangVien?> GetByIdAsync(int id);
        Task<GiangVien?> GetByIdWithLopHocsAsync(int id);
        Task<(IEnumerable<GiangVien> Items, int TotalCount)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize);
        Task AddAsync(GiangVien giangVien);
        Task UpdateAsync(GiangVien giangVien);
        Task DeleteAsync(int id);
        Task<bool> ExistsEmailAsync(string email, int? excludeId = null);
        Task<bool> CoLopHocLienQuanAsync(int maGiangVien);
        Task<bool> ExistsAsync(int id);
        Task SaveAsync();
    }
}
