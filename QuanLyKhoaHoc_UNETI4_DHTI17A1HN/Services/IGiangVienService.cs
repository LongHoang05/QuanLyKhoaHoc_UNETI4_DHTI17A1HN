// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tầng Service Giảng viên (Dữ liệu dùng chung)
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    public interface IGiangVienService
    {
        Task<List<GiangVien>> GetDanhSachAsync();
        Task<GiangVien?> GetByIdAsync(int id);
        Task<GiangVien?> GetByIdWithLopHocsAsync(int id);
        Task<(IEnumerable<GiangVien> Items, int TotalCount, int TotalPages, int PageIndex)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize);
        Task<(bool Success, string? ErrorMessage)> ThemGiangVienAsync(GiangVien giangVien);
        Task<(bool Success, string? ErrorMessage)> CapNhatGiangVienAsync(GiangVien giangVien);
        Task<(bool Success, string? ErrorMessage)> XoaGiangVienAsync(int id);
        Task<(bool Success, string? ErrorMessage)> DoiTrangThaiAsync(int id);
    }
}
