// Họ và tên: Nguyễn Đức Linh
// Mã sinh viên: 23103100042
// Nội dung thực hiện: Đăng ký lớp học và theo dõi đăng ký
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    public class DangKyHoc
    {
        [Key]
        public int MaDangKy { get; set; }

        // Foreign Key to HocVien (n - 1)
        [Required]
        public int MaHocVien { get; set; }
        [ForeignKey("MaHocVien")]
        public virtual HocVien? HocVien { get; set; }

        // Foreign Key to LopHoc (n - 1)
        [Required]
        public int MaLop { get; set; }
        [ForeignKey("MaLop")]
        public virtual LopHoc? LopHoc { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayDangKy { get; set; }

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? NgayXuLy { get; set; }

        [StringLength(500)]
        public string? NhanXetGiaoVu { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayDongHocPhi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn hoặc bằng 0")]
        public decimal? SoTienDaDong { get; set; }
    }
}
