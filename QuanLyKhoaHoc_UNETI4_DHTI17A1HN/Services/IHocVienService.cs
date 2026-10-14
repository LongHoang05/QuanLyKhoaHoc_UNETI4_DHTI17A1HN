using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh - MSV: 23103100042
    // Module 3: Tầng Service Quản lý Học viên
    // Đảm nhiệm toàn bộ quy tắc nghiệp vụ doanh nghiệp (Business Logic Layer)
    // Luồng: Controller -> Service -> Repository -> ApplicationDbContext
    // ========================================================
    public interface IHocVienService
    {
        /// <summary>
        /// Lấy toàn bộ danh sách học viên
        /// </summary>
        Task<List<HocVien>> GetDanhSachHocVienAsync();

        /// <summary>
        /// Tìm kiếm học viên phục vụ AJAX Live Search (Slide 389)
        /// </summary>
        Task<List<HocVien>> TimKiemHocVienAsync(string? keyword, string? searchType = null);

        /// <summary>
        /// Lấy chi tiết thông tin học viên kèm lịch sử đăng ký khóa học
        /// </summary>
        Task<HocVien?> GetChiTietHocVienAsync(int? id);

        /// <summary>
        /// Lấy thông tin học viên theo ID
        /// </summary>
        Task<HocVien?> GetHocVienByIdAsync(int? id);

        /// <summary>
        /// Nghiệp vụ thêm mới học viên: Thẩm định email duy nhất, kiểm tra tuổi hợp lệ (>= 6 tuổi)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> ThemHocVienAsync(HocVien hocVien);

        /// <summary>
        /// Nghiệp vụ cập nhật học viên: Kiểm tra tồn tại, chống trùng email với học viên khác, bảo vệ khóa định danh
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> CapNhatHocVienAsync(HocVien hocVien);

        /// <summary>
        /// Nghiệp vụ xóa học viên: Kiểm tra toàn vẹn CSDL, chặn xóa nếu học viên đã có lịch sử đăng ký học
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> XoaHocVienAsync(int id);

        /// <summary>
        /// Nghiệp vụ bật/tắt trạng thái hoạt động (Khóa / Mở khóa)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DoiTrangThaiAsync(int id);
    }
}
