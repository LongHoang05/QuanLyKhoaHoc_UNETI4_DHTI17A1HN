using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Bổ sung theo yêu cầu giảng viên hướng dẫn:
    // Repository Quản lý Giảng viên & Nghiệp vụ Giảng dạy (Repository Pattern)
    // ========================================================
    public interface IGiangVienRepository
    {
        // 1. Quản lý thực thể Giảng viên
        Task<List<GiangVien>> GetAllAsync();
        Task<GiangVien?> GetByIdAsync(int id);
        Task<GiangVien?> GetByTaiKhoanIdAsync(int maTaiKhoan);
        Task<GiangVien?> GetFirstAsync();
        Task<List<GiangVien>> SearchAsync(string? search, string? hocVi);
        Task AddAsync(GiangVien gv);
        Task UpdateAsync(GiangVien gv);
        Task DeleteAsync(int id);
        Task<bool> HasClassesAsync(int id);
        Task<bool> ExistsEmailAsync(string email, int? excludeId = null);

        // 2. Nghiệp vụ Lớp học & Buổi học của Giảng viên
        Task<List<LopHoc>> GetClassesByTeacherAsync(int maGiangVien, string teacherName);
        Task<List<BuoiHoc>> GetUpcomingSessionsAsync(int maGiangVien, List<int> classIds, int count = 6);
        Task<List<BuoiHoc>> GetSessionsInDateRangeAsync(int maGiangVien, List<int> classIds, DateTime fromDate, DateTime toDate, int? maLop = null);

        // 3. Nghiệp vụ Học viên & Chấm điểm
        Task<List<DangKyHoc>> GetClassStudentsWithGradesAsync(int maLop);
        Task<(bool Success, string Message, KetQuaHocTap? KetQua)> SaveGradeAsync(int maDangKy, double? diemCC, double? diemGK, double? diemCK, string? ghiChu);
        Task<int> CountGradedStudentsAsync(List<int> classIds);

        // 4. Quản lý Tài khoản liên kết
        Task<bool> ExistsUsernameAsync(string username);
        Task AddTaiKhoanAsync(TaiKhoan tk);

        Task SaveChangesAsync();
    }
}
