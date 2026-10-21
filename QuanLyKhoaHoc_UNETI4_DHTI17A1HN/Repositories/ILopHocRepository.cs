using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học & Tìm kiếm, Lọc, Sắp xếp, Phân trang LINQ
    // Interface Repository cho Entity LopHoc
    // ========================================================
    public interface ILopHocRepository
    {
        Task<IEnumerable<LopHoc>> GetAllAsync();
        Task<LopHoc?> GetByIdAsync(int id);
        Task<LopHoc?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(LopHoc lopHoc);
        Task UpdateAsync(LopHoc lopHoc);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasRegistrationsAsync(int id);
        Task<int> CountEnrolledAsync(int id);
        Task<IEnumerable<ChuongTrinhDaoTao>> GetChuongTrinhListAsync();

        /// <summary>
        /// Truy vấn LINQ: Tìm kiếm, Lọc đa điều kiện, Sắp xếp và Phân trang trên Server bằng Skip & Take
        /// </summary>
        Task<(IEnumerable<LopHoc> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            int? maChuongTrinh,
            string? trinhDo,
            string? trangThai,
            int? maGiangVien,
            decimal? minHocPhi,
            decimal? maxHocPhi,
            string? filterHanDangKy,
            string? filterConCho,
            string? sortBy,
            int pageIndex,
            int pageSize);

        /// <summary>
        /// Thống kê nhanh các chỉ số lớp học cho Dashboard / Header
        /// </summary>
        Task<(int TongSoLop, int SoLopDangMo, int SoLopDaDong, int SoLopTamDung)> GetThongKeNhanhAsync();
    }
}
