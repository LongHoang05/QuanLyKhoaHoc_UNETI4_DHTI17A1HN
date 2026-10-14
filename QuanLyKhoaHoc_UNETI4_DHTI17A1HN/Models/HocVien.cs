using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh
    // Mã sinh viên: 23103100042
    // Phụ trách: Module 3 - Quản lý học viên, Hồ sơ cá nhân, Đăng ký lớp học
    // ========================================================
    [Table("HocVien")]
    public class HocVien
    {
        [Key]
        [Display(Name = "Mã học viên")]
        public int MaHocVien { get; set; }

        [Display(Name = "Tài khoản liên kết")]
        public int? MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(200, ErrorMessage = "Họ tên không quá 200 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(200)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; } // Nam, Nữ, Khác

        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(100)]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDoHocVan { get; set; } // THPT, Đại học, Cao đẳng, Đi làm

        [StringLength(200)]
        [Display(Name = "Nghề nghiệp")]
        public string? NgheNghiep { get; set; }

        [StringLength(200)]
        [Display(Name = "Hình ảnh đại diện")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
    }
}
