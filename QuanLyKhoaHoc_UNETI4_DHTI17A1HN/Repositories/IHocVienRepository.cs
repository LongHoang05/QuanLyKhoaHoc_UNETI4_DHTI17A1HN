using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh - MSV: 23103100042
    // Module 3: Repository Quản Lý Học Viên
    // Chuẩn theo giáo trình bài giảng khoa CNTT UNETI (Slide 105, 119)
    // ========================================================
    public interface IHocVienRepository
    {
        // Lấy tất cả học viên (Slide 105)
        Task<List<HocVien>> GetAllAsync();
        List<HocVien> GetAll();

        // Lấy 1 học viên theo ID (Slide 105)
        Task<HocVien?> GetByIdAsync(int? id);

        // Lấy học viên kèm chi tiết tài khoản và lịch sử đăng ký khóa học
        Task<HocVien?> GetByIdWithDetailsAsync(int? id);

        // Lấy học viên theo mã tài khoản đăng nhập (Module 1)
        Task<HocVien?> GetByMaTaiKhoanAsync(int maTaiKhoan);

        // Thêm, sửa, xóa (Slide 105)
        Task AddAsync(HocVien hocVien);
        Task UpdateAsync(HocVien hocVien);
        Task DeleteAsync(int id);

        // Kiểm tra tồn tại
        bool Exists(int id);
        Task<bool> ExistsEmailAsync(string email, int? excludeId = null);
        Task<bool> CoDangKyHocAsync(int maHocVien);
        Task SaveAsync();

        // Tìm kiếm LINQ phục vụ AJAX Search (Theo Slide 119 - 121 & Slide 389)
        Task<List<HocVien>> SearchAsync(string? keyword, string? searchType = null);
        Task<List<HocVien>> SearchTrangThaiAsync(bool? trangThai);

        // Phân trang LINQ Skip/Take
        Task<(IEnumerable<HocVien> Items, int TotalCount)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize);
    }
}
