using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học, Tìm kiếm, Lọc, Sắp xếp, Phân trang LINQ
    // Service Interface xử lý toàn bộ logic nghiệp vụ Module 2
    // ========================================================
    public interface ILopHocService
    {
        Task<LopHocListViewModel> GetDanhSachLopAsync(
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
            int pageSize = 8);

        Task<LopHoc?> GetChiTietLopAsync(int id);
        Task<(bool Success, string? ErrorMessage)> TaoLopHocAsync(LopHoc lopHoc);
        Task<(bool Success, string? ErrorMessage)> CapNhatLopHocAsync(LopHoc lopHoc);
        Task<(bool Success, string? ErrorMessage)> XoaLopHocAsync(int id);

        Task<IEnumerable<ChuongTrinhDaoTao>> GetDanhSachChuongTrinhAsync();
        Task<IEnumerable<GiangVien>> GetDanhSachGiangVienAsync();
        Task<int> GetSoLuongHocVienDaDuyetAsync(int maLopHoc);
    }
}
