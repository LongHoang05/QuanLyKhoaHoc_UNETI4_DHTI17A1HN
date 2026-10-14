using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    /// <summary>
    /// Module 1 - Vũ Đình Trường (23103100026)
    /// Bảng Khóa học
    /// </summary>
    [Table("KhoaHoc")]
    public class KhoaHoc
    {
        [Key]
        public int MaKhoaHoc { get; set; }

        [Required(ErrorMessage = "Tên khóa học không được để trống")]
        [StringLength(300)]
        [Display(Name = "Tên khóa học")]
        public string TenKhoaHoc { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Danh mục")]
        public int MaDanhMuc { get; set; }

        [Required(ErrorMessage = "Học phí không được để trống")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Học phí")]
        public decimal HocPhi { get; set; }

        [Display(Name = "Thời lượng (giờ)")]
        public int ThoiLuong { get; set; }

        [StringLength(200)]
        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaDanhMuc")]
        public virtual DanhMucKhoaHoc? DanhMucKhoaHoc { get; set; }

        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
    }
}
