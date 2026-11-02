// ========================================================
// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Phụ trách: Module 5 - Buổi học, Điểm danh, Kết quả học tập & Thống kê báo cáo
// Bài tập thực hành: Repository Pattern - UNETI
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface IKetQuaHocTapRepository
    {
        // 1. Các phương thức CRUD cơ bản theo chuẩn Repository Pattern
        Task<IEnumerable<KetQuaHocTap>> GetAllAsync();
        Task<KetQuaHocTap?> GetByIdAsync(int id);
        Task<KetQuaHocTap?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(KetQuaHocTap item);
        Task UpdateAsync(KetQuaHocTap item);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task SaveAsync();

        // 2. Các phương thức truy vấn LINQ: Tìm kiếm, Lọc, Sắp xếp, Phân trang
        Task<(IEnumerable<KetQuaHocTap> Items, int TotalCount)> GetPagedAsync(
            string? search,
            int? maLop,
            string? xepLoai,
            string? ketQua,
            int page,
            int pageSize);

        // 3. Thống kê phân phối điểm & tỷ lệ đạt/trượt
        Task<(int TongSoDiem, int SoDat, int SoKhongDat, double DiemTrungBinh)> GetThongKePhoiDiemAsync();

        // 4. Các truy vấn theo hồ sơ & lớp học
        Task<bool> ExistsByDangKyIdAsync(int maDangKy);
        Task<KetQuaHocTap?> GetByDangKyIdAsync(int maDangKy);
        Task<IEnumerable<DangKyHoc>> GetEligibleDangKysForGradingAsync(int? maLopHoc = null);
        Task<IEnumerable<KetQuaHocTap>> GetByHocVienIdAsync(int maHocVien);
        Task<IEnumerable<KetQuaHocTap>> GetByLopIdAsync(int maLopHoc);
    }
}

