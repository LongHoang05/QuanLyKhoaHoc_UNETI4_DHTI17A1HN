// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// Bài tập thực hành: Repository Pattern - UNETI
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface IChuongTrinhDaoTaoRepository
    {
        // 1. Các phương thức CRUD cơ bản theo chuẩn Repository Pattern
        Task<List<ChuongTrinhDaoTao>> GetAllAsync();
        Task<ChuongTrinhDaoTao?> GetByIdAsync(int id);
        Task<ChuongTrinhDaoTao?> GetByIdWithLopHocsAsync(int id);
        Task AddAsync(ChuongTrinhDaoTao item);
        Task UpdateAsync(ChuongTrinhDaoTao item);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task SaveAsync();

        // 2. Truy vấn LINQ: Tìm kiếm, lọc và phân trang
        Task<(IEnumerable<ChuongTrinhDaoTao> Items, int TotalCount)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize);

        // 3. Ràng buộc nghiệp vụ & Thống kê
        Task<bool> ExistsTenChuongTrinhAsync(string tenChuongTrinh, int? excludeId = null);
        Task<bool> CoLopHocLienQuanAsync(int maChuongTrinh);
        Task<int> CountLopHocByChuongTrinhAsync(int maChuongTrinh);
        Task<Dictionary<int, int>> GetClassCountsByProgramAsync(IEnumerable<int> ctdtIds);
        Task<(int TongCTDT, int DangHoatDong, int TongLopLienKet)> GetKpiStatsAsync();
    }
}
