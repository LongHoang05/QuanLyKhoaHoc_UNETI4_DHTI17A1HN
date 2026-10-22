// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using System.ComponentModel.DataAnnotations;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels
{
    /// <summary>
    /// ViewModel dùng cho form Đăng nhập
    /// </summary>
    public class DangNhapViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool GhiNho { get; set; } = false;

        public string? ReturnUrl { get; set; }
    }

    /// <summary>
    /// ViewModel dùng cho form Đăng ký học viên mới
    /// </summary>
    public class DangKyViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 100 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_.]+$", ErrorMessage = "Tên đăng nhập chỉ chứa chữ cái, số, dấu chấm hoặc gạch dưới")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(200, ErrorMessage = "Họ tên không vượt quá 200 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [StringLength(200)]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string XacNhanMatKhau { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel dùng cho chức năng Đổi mật khẩu
    /// </summary>
    public class DoiMatKhauViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string MatKhauCu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string MatKhauMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp với mật khẩu mới")]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string XacNhanMatKhauMoi { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel dùng cho xem và cập nhật hồ sơ cá nhân
    /// </summary>
    public class HoSoCaNhanViewModel
    {
        public int MaTaiKhoan { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(200)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(200)]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Vai trò người dùng")]
        public string VaiTro { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDoHocVan { get; set; }

        [Display(Name = "Nghề nghiệp")]
        public string? NgheNghiep { get; set; }

        [Display(Name = "Ngày tham gia")]
        public DateTime NgayTao { get; set; }
    }

    /// <summary>
    /// ViewModel dùng cho màn hình danh sách tài khoản của Admin (Tìm kiếm + Lọc + Phân trang)
    /// </summary>
    public class TaiKhoanListViewModel
    {
        public IEnumerable<TaiKhoan> Items { get; set; } = new List<TaiKhoan>();
        public string? Keyword { get; set; }
        public string? VaiTro { get; set; }
        public bool? TrangThai { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageNumber => PageIndex;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
