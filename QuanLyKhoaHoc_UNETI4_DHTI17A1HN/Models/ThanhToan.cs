using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    /// <summary>
    /// Module 5 - Nguyễn Tùng Dương (23103100003)
    /// Bảng Thanh toán
    /// </summary>
    [Table("ThanhToan")]
    public class ThanhToan
    {
        [Key]
        public int MaThanhToan { get; set; }

        [Display(Name = "Đăng ký học")]
        public int MaDangKy { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số tiền")]
        public decimal SoTien { get; set; }

        [Display(Name = "Ngày thanh toán")]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        [StringLength(50)]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = "TienMat"; // TienMat, ChuyenKhoan, TheNganHang

        [StringLength(50)]
        [Display(Name = "Trạng thái thanh toán")]
        public string TrangThaiThanhToan { get; set; } = "ChuaThanhToan"; // ChuaThanhToan, DaThanhToan, HoanTien

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // Navigation property
        [ForeignKey("MaDangKy")]
        public virtual DangKyHoc? DangKyHoc { get; set; }
    }
}
