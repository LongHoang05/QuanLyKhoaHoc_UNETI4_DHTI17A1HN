using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học & Giảng viên
    // Interface Repository cho Entity GiangVien
    // ========================================================
    public interface IGiangVienRepository
    {
        Task<IEnumerable<GiangVien>> GetAllAsync();
        Task<GiangVien?> GetByIdAsync(int id);
        Task AddAsync(GiangVien giangVien);
        Task UpdateAsync(GiangVien giangVien);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
