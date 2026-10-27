using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    // ========================================================
    // Bổ sung theo yêu cầu giảng viên hướng dẫn:
    // Service Interface cho Cổng Giảng viên & Quản lý Giảng viên (Service Pattern)
    // ========================================================
    public interface IGiangVienService
    {
        // 1. Xác định giảng viên hiện tại theo Session
        Task<GiangVien?> GetCurrentGiangVienAsync(int? maGiangVien, int? maTaiKhoan);

        // 2. Nghiệp vụ Dashboard Giảng viên
        Task<List<LopHoc>> GetClassesForTeacherAsync(GiangVien gv);
        Task<List<BuoiHoc>> GetUpcomingSessionsAsync(GiangVien gv, int take = 6);
        Task<int> GetGradedCountAsync(List<int> classIds);

        // 3. Nghiệp vụ Lịch giảng dạy / Thời khóa biểu
        Task<List<BuoiHoc>> GetWeeklyTimetableAsync(GiangVien gv, int? maLop, DateTime weekStart);

        // 4. Nghiệp vụ Chấm điểm học viên
        Task<List<DangKyHoc>> GetStudentsForGradingAsync(int maLop);
        Task<(bool Success, string Message, object? Data)> LuuDiemHocVienAsync(int maDangKy, double? diemCC, double? diemGK, double? diemCK, string? ghiChu);

        // 5. Nghiệp vụ Quản trị Giảng viên (Admin & Giáo vụ)
        Task<List<GiangVien>> GetDanhSachGiangVienAsync(string? search, string? hocVi);
        Task<GiangVien?> GetGiangVienByIdAsync(int id);
        Task<(bool Success, string Message)> TaoGiangVienAsync(GiangVien gv, string? tenDangNhap, string? matKhau);
        Task<(bool Success, string Message)> CapNhatGiangVienAsync(int maGiangVien, GiangVien gv);
        Task<(bool Success, string Message)> XoaGiangVienAsync(int id);
    }
}
