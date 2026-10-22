using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh - MSV: 23103100042
    // Module 3: Repository Quản Lý Học Viên & Tài Khoản (Mục 5 & 7)
    // Chuẩn theo giáo trình bài giảng khoa CNTT UNETI (Slide 105, 119)
    // ========================================================
    public interface IHocVienRepository
    {
        #region 1. Nghiệp vụ Thực thể Học Viên (Entity HocVien)
        // Lấy tất cả học viên kèm thông tin lớp và tài khoản
        Task<List<HocVien>> GetAllAsync();
        List<HocVien> GetAll();

        // Lấy 1 học viên theo ID
        Task<HocVien?> GetByIdAsync(int? id);

        // Lấy học viên kèm chi tiết tài khoản và lịch sử đăng ký khóa học
        Task<HocVien?> GetByIdWithDetailsAsync(int? id);

        // Lấy học viên theo mã tài khoản đăng nhập (Module 1)
        Task<HocVien?> GetByMaTaiKhoanAsync(int maTaiKhoan);

        // Thêm, sửa, xóa học viên
        Task AddAsync(HocVien hocVien);
        Task UpdateAsync(HocVien hocVien);
        Task DeleteAsync(int id);

        // Kiểm tra tồn tại
        bool Exists(int id);
        Task<bool> ExistsEmailAsync(string email, int? excludeId = null);
        Task<bool> CoDangKyHocAsync(int maHocVien);
        Task SaveAsync();

        // Tìm kiếm và Lọc học viên đa tiêu chí: Từ khóa (Tên, Email, SĐT), Trạng thái, và LỌC THEO LỚP HỌC
        Task<List<HocVien>> SearchHocVienAsync(string? keyword, bool? trangThai = null, int? maLopHoc = null);

        // Phân trang LINQ Skip/Take
        Task<(IEnumerable<HocVien> Items, int TotalCount)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize);
        #endregion

        #region 2. Nghiệp vụ Thực thể Tài Khoản (Entity TaiKhoan) & Lớp Học
        // Lấy danh sách tài khoản
        Task<List<TaiKhoan>> GetAllTaiKhoanAsync();

        // Tìm kiếm và lọc tài khoản: Từ khóa, Vai trò, Trạng thái hoạt động
        Task<List<TaiKhoan>> SearchTaiKhoanAsync(string? keyword, string? vaiTro = null, bool? trangThai = null);

        // Lấy tài khoản theo ID
        Task<TaiKhoan?> GetTaiKhoanByIdAsync(int id);

        // Kiểm tra tồn tại tên đăng nhập
        Task<bool> ExistsUsernameAsync(string username);

        // Thêm mới tài khoản
        Task AddTaiKhoanAsync(TaiKhoan taiKhoan);

        // Cập nhật tài khoản (Khóa/Mở khóa, Đổi mật khẩu)
        Task UpdateTaiKhoanAsync(TaiKhoan taiKhoan);

        // Lấy danh sách toàn bộ Lớp học để hiển thị dropdown lọc theo lớp
        Task<List<LopHoc>> GetAllLopHocAsync();
        #endregion
    }
}
