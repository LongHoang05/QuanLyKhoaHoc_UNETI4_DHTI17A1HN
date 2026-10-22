using System.ComponentModel.DataAnnotations;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh
    // Mã sinh viên: 23103100042
    // Phụ trách: Module 3 - ViewModels Quản lý Học viên
    // ========================================================

    /// <summary>
    /// ViewModel đại diện cho 1 dòng học viên trong bảng danh sách
    /// </summary>
    public class HocVienItemViewModel
    {
        [Display(Name = "Mã HV")]
        public int MaHocVien { get; set; }

        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Display(Name = "Trình độ")]
        public string? TrinhDoHocVan { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; }

        [Display(Name = "Ngày tham gia")]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Tài khoản liên kết")]
        public string? TenDangNhap { get; set; }

        [Display(Name = "Lớp đã đăng ký")]
        public int SoLopDaDangKy { get; set; }
    }

    /// <summary>
    /// ViewModel chứa danh sách học viên, bộ lọc tìm kiếm và dữ liệu phân trang
    /// </summary>
    public class HocVienListViewModel
    {
        public IEnumerable<HocVienItemViewModel> Items { get; set; } = new List<HocVienItemViewModel>();

        public string? Keyword { get; set; }

        public bool? TrangThai { get; set; }

        public int PageIndex { get; set; } = 1;

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;
    }

    /// <summary>
    /// ViewModel chi tiết học viên kèm lịch sử đăng ký khóa học
    /// </summary>
    public class HocVienDetailViewModel
    {
        [Display(Name = "Mã học viên")]
        public int MaHocVien { get; set; }

        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDoHocVan { get; set; }

        [Display(Name = "Nghề nghiệp")]
        public string? NgheNghiep { get; set; }

        [Display(Name = "Hình ảnh đại diện")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; }

        [Display(Name = "Ngày tạo hồ sơ")]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Mã tài khoản")]
        public int? MaTaiKhoan { get; set; }

        [Display(Name = "Tên đăng nhập liên kết")]
        public string? TenDangNhap { get; set; }

        // Danh sách các lớp học viên đã từng đăng ký
        public List<DangKyHocLichSuItemViewModel> DanhSachDangKy { get; set; } = new();
    }

    /// <summary>
    /// Item hiển thị một đăng ký của học viên trong màn hình chi tiết
    /// </summary>
    public class DangKyHocLichSuItemViewModel
    {
        public int MaDangKy { get; set; }
        public string TenLop { get; set; } = string.Empty;
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string TenChuongTrinh { get; set; } = string.Empty;
        public DateTime NgayDangKy { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public decimal HocPhi { get; set; }
        public decimal SoTienDaDong { get; set; }
        public string? NhanXetGiaoVu { get; set; }
    }

    /// <summary>
    /// ViewModel form tạo mới học viên
    /// </summary>
    public class HocVienCreateViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [StringLength(200, ErrorMessage = "Email không được vượt quá 200 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại không quá 15 ký tự")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; } = "Nam";

        [StringLength(500, ErrorMessage = "Địa chỉ không quá 500 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(100)]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDoHocVan { get; set; } = "Đại học";

        [StringLength(200, ErrorMessage = "Nghề nghiệp không quá 200 ký tự")]
        [Display(Name = "Nghề nghiệp")]
        public string? NgheNghiep { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;
    }

    /// <summary>
    /// ViewModel form cập nhật thông tin học viên
    /// </summary>
    public class HocVienEditViewModel
    {
        [Required]
        public int MaHocVien { get; set; }

        public int? MaTaiKhoan { get; set; }

        [Display(Name = "Tài khoản liên kết")]
        public string? TenDangNhap { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [StringLength(200, ErrorMessage = "Email không được vượt quá 200 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại không quá 15 ký tự")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ không quá 500 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(100)]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDoHocVan { get; set; }

        [StringLength(200, ErrorMessage = "Nghề nghiệp không quá 200 ký tự")]
        [Display(Name = "Nghề nghiệp")]
        public string? NgheNghiep { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; }
    }

    /// <summary>
    /// ViewModel dùng cho Cổng Thông Tin Học Viên (Portal Học Viên - Chuẩn Giao_DIen_Test hoc-vien.html)
    /// </summary>
    public class CongHocVienViewModel
    {
        public HocVien HocVien { get; set; } = new HocVien();
        public List<LopHoc> LopHocDangMos { get; set; } = new List<LopHoc>();
        public List<DangKyHoc> DanhSachDangKy { get; set; } = new List<DangKyHoc>();
        public List<ChuongTrinhDaoTao> ChuongTrinhs { get; set; } = new List<ChuongTrinhDaoTao>();
    }

    /// <summary>
    /// ViewModel chính cho trang Index Module 3 (Quản lý 2 Tab: TaiKhoan & HocVien chuẩn admin-tai-khoan.html)
    /// </summary>
    public class HocVienIndexViewModel
    {
        // Danh sách tài khoản cho Tab 1
        public List<TaiKhoan> DanhSachTaiKhoan { get; set; } = new();

        // Danh sách hồ sơ học viên cho Tab 2
        public List<HocVien> DanhSachHocVien { get; set; } = new();

        // Danh sách lớp học để đổ vào dropdown lọc theo lớp ở Tab 2
        public List<LopHoc> DanhSachLopHoc { get; set; } = new();

        // 4 Chỉ số mini stat cards
        public int TongSoHocVien { get; set; }
        public int SoGiaoVu { get; set; }
        public int SoAdmin { get; set; }
        public int SoBiKhoa { get; set; }

        // Tab hiện tại đang chọn: 'accounts' (Tab 1) hoặc 'profiles' (Tab 2)
        public string CurrentTab { get; set; } = "accounts";

        // Mã lớp học đang lọc (nếu có)
        public int? SelectedMaLopHoc { get; set; }
    }

    /// <summary>
    /// ViewModel cho Modal Thêm Tài Khoản Mới (Entity TaiKhoan + HocVien)
    /// </summary>
    public class ThemTaiKhoanViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(100, ErrorMessage = "Tên đăng nhập không quá 100 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = "123456";

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(200, ErrorMessage = "Họ tên không quá 200 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(200)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "HocVien"; // Admin, GiaoVu, HocVien
    }

    /// <summary>
    /// DTO chi tiết học viên trả về qua JSON để hiển thị Modal chuẩn admin-tai-khoan.html
    /// </summary>
    public class HocVienChiTietModalDto
    {
        public int MaHocVien { get; set; }
        public int? MaTaiKhoan { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string? SoDienThoai { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public string? TrinhDoHocVan { get; set; }
        public string? NgheNghiep { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string LopHocDangKy { get; set; } = "Chưa đăng ký lớp";
    }
}
