// ========================================================
// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Phụ trách: Module 4 - Đăng ký khóa học, Quản lý hồ sơ & Thu học phí
// Bài tập thực hành: Repository Pattern - UNETI
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface IDangKyHocRepository
    {
        // 1. Các phương thức CRUD cơ bản theo chuẩn Repository Pattern
        Task<IEnumerable<DangKyHoc>> GetAllAsync();
        Task<DangKyHoc?> GetByIdAsync(int id);
        Task<DangKyHoc?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(DangKyHoc item);
        Task UpdateAsync(DangKyHoc item);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task SaveAsync();

        // 2. Các phương thức truy vấn LINQ: Tìm kiếm, Lọc, Sắp xếp, Phân trang
        Task<(IEnumerable<DangKyHoc> Items, int TotalCount)> GetPagedAsync(
            string? search,
            int? maChuongTrinh,
            int? maLop,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sortBy,
            int page,
            int pageSize);

        // 3. Thống kê nhanh chỉ số đăng ký học
        Task<(int ChoDuyetCount, int DaDuyetCount, double TongDoanhThu)> GetThongKeNhanhAsync();

        // 4. Các truy vấn nghiệp vụ đăng ký & học viên
        Task<int> CountEnrolledInLopAsync(int maLop);
        Task<bool> HasEnrolledAsync(int maHocVien, int maLop);
        Task<DangKyHoc?> GetActiveRegistrationAsync(int maHocVien, int maLop);
        Task<IEnumerable<DangKyHoc>> GetByHocVienIdAsync(int maHocVien);
        Task<IEnumerable<DangKyHoc>> GetPaymentHistoryByHocVienIdAsync(int maHocVien);
        Task<Dictionary<int, int>> GetSiSoDaDangKyMapAsync(IEnumerable<int> maLopList);
        Task<Dictionary<int, int>> GetAllSiSoMapAsync();
        Task<int> GetTotalCountExcludingCancelledAsync();
    }
}

