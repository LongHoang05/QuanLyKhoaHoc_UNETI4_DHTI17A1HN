// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - Entity HocVien

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities
{
    public class HocVien
    {
        [Key]
        public int MaHocVien { get; set; }

        [ForeignKey("TaiKhoan")]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = "";

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        // Quy ước cả nhóm: true = Nam, false = Nữ
        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        [Display(Name = "Giới tính")]
        public bool? GioiTinh { get; set; }

        [RegularExpression(@"^(0|\+84)(3|5|7|8|9)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ (ví dụ 0901234567)")]
        [MaxLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(250)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [MaxLength(100)]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDoHocVan { get; set; }

        [MaxLength(100)]
        [Display(Name = "Nghề nghiệp")]
        public string? NgheNghiep { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Hoạt động";

        // Thuộc tính điều hướng
        public TaiKhoan? TaiKhoan { get; set; }
        public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
    }
}