// ========================================================
// Họ và tên: Nguyễn Đình Kiên
// Mã sinh viên: 23103100020
// Phụ trách: Module 2 - Quản lý lớp học, Tìm kiếm, Lọc, Sắp xếp và Phân trang
// Bài tập thực hành: Repository Pattern - UNETI
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface ILopHocRepository
    {
        // 1. Các phương thức CRUD cơ bản theo chuẩn Repository Pattern
        Task<IEnumerable<LopHoc>> GetAllAsync();
        Task<LopHoc?> GetByIdAsync(int id);
        Task<LopHoc?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(LopHoc lopHoc);
        Task UpdateAsync(LopHoc lopHoc);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task SaveAsync();

        // 2. Nghiệp vụ kiểm tra ràng buộc
        Task<bool> HasRegistrationsAsync(int id);
        Task<int> CountEnrolledAsync(int id);
        Task<bool> ExistsTenLopAsync(string tenLop, int? excludeId = null);

        // 3. Danh mục hỗ trợ Dropdown Filter
        Task<IEnumerable<ChuongTrinhDaoTao>> GetChuongTrinhListAsync();
        Task<IEnumerable<string>> GetDanhSachTenGiangVienAsync();

        // 4. Truy vấn LINQ: Tìm kiếm, Lọc đa điều kiện, Sắp xếp và Phân trang trên Server bằng Skip & Take
        Task<(IEnumerable<LopHoc> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            int? maChuongTrinh,
            string? trinhDo,
            string? trangThai,
            string? giangVien,
            decimal? minHocPhi,
            decimal? maxHocPhi,
            string? filterHanDangKy,
            string? filterConCho,
            string? sortBy,
            int pageIndex,
            int pageSize);

        // 5. Thống kê & Portal Trang chủ / Cổng học viên
        Task<(int TongSoLop, int SoLopDangMo, int SoLopDaDong, int SoLopTamDung)> GetThongKeNhanhAsync();
        Task<int> CountOpenClassesAsync();
        Task<int> CountTotalClassesAsync();
        Task<IEnumerable<LopHoc>> GetHotCoursesAsync(int take = 4);
        Task<IEnumerable<LopHoc>> GetUpcomingCoursesAsync(int take = 4);
        Task<IEnumerable<LopHoc>> GetSimilarCoursesAsync(int maChuongTrinh, int excludeMaLop, int take = 3);
        Task<Dictionary<int, int>> GetOpenClassCountsByProgramAsync();

        // 6. Phương thức tìm kiếm AJAX theo bài thực hành Javascript - jQuery - AJAX (Bài 11 - Bài 15)
        Task<IEnumerable<LopHoc>> SearchAjaxAsync(string? keyword, int? maChuongTrinh = null);
    }
}
