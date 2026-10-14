using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Vũ Đình Trường
    // Mã sinh viên: 23103100026
    // Phụ trách: Module 1 - Quản lý Chương trình đào tạo / Danh mục khóa học
    // ========================================================
    [Table("DanhMucKhoaHoc")]
    public class DanhMucKhoaHoc
    {
        [Key]
        [Display(Name = "Mã chương trình")]
        public int MaDanhMuc { get; set; }

        [Required(ErrorMessage = "Tên chương trình/danh mục không được để trống")]
        [StringLength(200, ErrorMessage = "Tên không vượt quá 200 ký tự")]
        [Display(Name = "Tên chương trình đào tạo")]
        public string TenDanhMuc { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [EmailAddress(ErrorMessage = "Email liên hệ không đúng định dạng")]
        [StringLength(200)]
        [Display(Name = "Email liên hệ")]
        public string? EmailLienHe { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();
    }
}
