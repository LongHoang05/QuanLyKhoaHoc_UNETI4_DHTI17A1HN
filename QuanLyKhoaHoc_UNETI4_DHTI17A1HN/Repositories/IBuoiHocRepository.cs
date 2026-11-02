// ========================================================
// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Phụ trách: Module 5 - Buổi học, Điểm danh, Kết quả học tập & Thống kê báo cáo
// Bài tập thực hành: Repository Pattern - UNETI
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface IBuoiHocRepository
    {
        // 1. Các phương thức CRUD cơ bản theo chuẩn Repository Pattern
        Task<IEnumerable<BuoiHoc>> GetAllAsync();
        Task<BuoiHoc?> GetByIdAsync(int id);
        Task<BuoiHoc?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(BuoiHoc item);
        Task UpdateAsync(BuoiHoc item);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task SaveAsync();

        // 2. Các phương thức truy vấn LINQ: Tìm kiếm, Lọc, Sắp xếp, Phân trang
        Task<(IEnumerable<BuoiHoc> Items, int TotalCount)> GetPagedAsync(
            string? search,
            int? maLop,
            string? phongHoc,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sortBy,
            int page,
            int pageSize);

        // 3. Thống kê số lượng buổi học
        Task<(int TongSoBuoi, int DaHoanThanh, int DaLenLich, int DaHuy)> GetThongKeBuoiHocAsync();

        // 4. Các truy vấn phục vụ kiểm tra trùng lịch & ràng buộc
        Task<int> CountBuoiHocChuaHuyAsync(int maLopHoc);
        Task<Dictionary<int, int>> GetBuoiHocChuaHuyMapAsync(IEnumerable<int> lopIds);
        Task<bool> KiemTraTrungLichPhongAsync(string phongHoc, DateTime batDau, DateTime ketThuc, int? excludeId = null);
        Task<bool> KiemTraTrungLichGiangVienAsync(int maGiangVien, DateTime batDau, DateTime ketThuc, int? excludeId = null);
        Task<bool> KiemTraTrungLichBuoiHocAsync(int maLopHoc, string phongHoc, string? tenGiangVien, DateTime batDau, DateTime ketThuc, int? excludeId = null);
        Task<(HashSet<int> ConflictIds, int SoLoiTrung)> GetConflictSessionsAsync();
        Task<bool> HasLichHocLopChuaHuyAsync(int maLopHoc);

        // 5. Truy vấn theo lớp học & học viên
        Task<IEnumerable<BuoiHoc>> GetByLopHocIdAsync(int maLopHoc);
        Task<IEnumerable<BuoiHoc>> GetBuoiHocsByLopIdsAsync(IEnumerable<int> lopHocIds);
        Task<BuoiHoc?> GetNextBuoiHocForLopAsync(int maLopHoc);
        Task<int> CountCompletedSessionsForLopAsync(int maLopHoc);
    }
}
