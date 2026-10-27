using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Vũ Đình Trường
    // Mã sinh viên: 23103100026
    // Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
    // ========================================================
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(100, ErrorMessage = "Tên đăng nhập không quá 100 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(500, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(200, ErrorMessage = "Họ tên không quá 200 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(200)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "HocVien"; // Admin, GiaoVu, GiangVien, HocVien

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true; // true = Hoạt động, false = Bị khóa

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation property liên kết 1-1 với Học viên nếu vai trò là Học viên
        public virtual HocVien? HocVien { get; set; }

        // Navigation property liên kết 1-1 với Giảng viên nếu vai trò là Giảng viên
        public virtual GiangVien? GiangVien { get; set; }
    }
}
