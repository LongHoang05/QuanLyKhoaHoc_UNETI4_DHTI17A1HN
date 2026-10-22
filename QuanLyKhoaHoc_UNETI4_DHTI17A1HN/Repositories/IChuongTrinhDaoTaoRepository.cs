// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface IChuongTrinhDaoTaoRepository
    {
        Task<List<ChuongTrinhDaoTao>> GetAllAsync();
        Task<ChuongTrinhDaoTao?> GetByIdAsync(int id);
        Task<ChuongTrinhDaoTao?> GetByIdWithLopHocsAsync(int id);
        Task<(IEnumerable<ChuongTrinhDaoTao> Items, int TotalCount)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize);
        Task AddAsync(ChuongTrinhDaoTao item);
        Task UpdateAsync(ChuongTrinhDaoTao item);
        Task DeleteAsync(int id);
        Task<bool> ExistsTenChuongTrinhAsync(string tenChuongTrinh, int? excludeId = null);
        Task<bool> CoLopHocLienQuanAsync(int maChuongTrinh);
        Task<bool> ExistsAsync(int id);
        Task SaveAsync();
    }
}
