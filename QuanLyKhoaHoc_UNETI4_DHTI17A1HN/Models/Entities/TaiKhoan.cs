// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - Entity TaiKhoan

using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = "";

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MaxLength(200)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = "";

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = "";

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "";

        [Required]
        [MaxLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Hoạt động";

        // Thuộc tính điều hướng: 1 tài khoản có 0 hoặc 1 hồ sơ học viên
        public HocVien? HocVien { get; set; }
    }
}