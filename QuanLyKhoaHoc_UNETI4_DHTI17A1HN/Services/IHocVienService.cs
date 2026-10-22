using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh
    // Mã sinh viên: 23103100042
    // Phụ trách: Module 3 - Quản lý học viên, Hồ sơ cá nhân và Đăng ký học
    // ========================================================
    public interface IHocVienService
    {
        #region 1. Nghiệp vụ Học Viên (HocVien)
        /// <summary>
        /// Lấy toàn bộ danh sách học viên
        /// </summary>
        Task<List<HocVien>> GetDanhSachHocVienAsync();

        /// <summary>
        /// Tìm kiếm học viên phục vụ AJAX Live Search: Từ khóa, Trạng thái, và Lọc theo Lớp Học
        /// </summary>
        Task<List<HocVien>> TimKiemHocVienAsync(string? keyword, bool? trangThai = null, int? maLopHoc = null);

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
        /// Nghiệp vụ bật/tắt trạng thái hoạt động (Khóa / Mở khóa học viên)
        /// </summary>
        Task<(bool Success, string? ErrorMessage, bool NewStatus)> DoiTrangThaiAsync(int id);
        #endregion

        #region 2. Nghiệp vụ Tài Khoản (TaiKhoan) & Lớp Học
        /// <summary>
        /// Lấy danh sách tài khoản cho Tab 1
        /// </summary>
        Task<List<TaiKhoan>> GetDanhSachTaiKhoanAsync();

        /// <summary>
        /// Tìm kiếm và lọc tài khoản theo từ khóa, vai trò và trạng thái
        /// </summary>
        Task<List<TaiKhoan>> TimKiemTaiKhoanAsync(string? keyword, string? vaiTro = null, bool? trangThai = null);

        /// <summary>
        /// Khóa / Mở khóa tài khoản (Entity TaiKhoan - Mục 5.1)
        /// </summary>
        Task<(bool Success, string? ErrorMessage, bool NewStatus, string Username)> DoiTrangThaiTaiKhoanAsync(int id);

        /// <summary>
        /// Cấp lại mật khẩu cho tài khoản
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> ResetMatKhauTaiKhoanAsync(int id, string? newPassword);

        /// <summary>
        /// Thêm mới tài khoản và tự động tạo hồ sơ học viên tương ứng nếu vai trò là Học viên
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> ThemTaiKhoanMoiAsync(ThemTaiKhoanViewModel model);

        /// <summary>
        /// Lấy danh sách toàn bộ lớp học cho dropdown lọc
        /// </summary>
        Task<List<LopHoc>> GetDanhSachLopHocAsync();

        /// <summary>
        /// Lấy 4 chỉ số thống kê cho 4 mini stat cards trên đầu trang
        /// </summary>
        Task<(int TongHocVien, int SoGiaoVu, int SoAdmin, int SoBiKhoa)> GetThongKeAsync();
        #endregion
    }
}
