using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    /// <summary>
    /// Module 2 - Nguyễn Đình Kiên (23103100020)
    /// Bảng Giảng viên
    /// </summary>
    [Table("GiangVien")]
    public class GiangVien
    {
        [Key]
        public int MaGiangVien { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(200)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(200)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [StringLength(500)]
        [Display(Name = "Chuyên ngành")]
        public string? ChuyenNganh { get; set; }

        [StringLength(200)]
        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
    }
}
