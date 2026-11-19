// ========================================================
// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Phụ trách: Module 4 - Đăng ký khóa học, Quản lý hồ sơ & Thu học phí
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    public interface IDangKyHocService
    {
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

        Task<(int ChoDuyetCount, int DaDuyetCount, double TongDoanhThu)> GetThongKeNhanhAsync();
        Task<DangKyHoc?> GetByIdWithDetailsAsync(int id);

        Task<(bool Success, string Message)> DuyetDangKyAsync(int id, string? nhanXet, string nguoiDuyet);
        Task<(bool Success, string Message)> XacNhanHocPhiAsync(int id, string? phuongThuc, string? ghiChu, string staffName);
        Task<(bool Success, string Message)> HoanTacChuaNhanHocPhiAsync(int id, string? ghiChu, string staffName);
        Task<(bool Success, string Message)> BatDauHocAsync(int id);
        Task<(bool Success, string Message)> TuChoiDangKyAsync(int id, string lyDo, string staffName);
        Task<(bool Success, string Message)> HuyDangKyAsync(int id, string? lyDo, string? vaiTro, string staffName);
        Task<(bool Success, string Message)> XoaDangKyAsync(int id);
        Task<(bool Success, string Message, int DeletedCount, int SkippedCount)> XoaNhieuDangKyAsync(IEnumerable<int>? ids);
    }
}
