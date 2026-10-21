using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Nguyễn Hoàng Long
    // Mã sinh viên: 23103100046
    // Phụ trách: Module 4 - Tiếp nhận, Xét duyệt, Học phí và Quản lý trạng thái đăng ký
    // Entity DangKyHoc - Chuẩn Mục 7.2 & 8.4 Đề tài BTL UNETI
    // ========================================================
    [Table("DangKyHoc")]
    public class DangKyHoc
    {
        [Key]
        [Display(Name = "Mã đăng ký")]
        public int MaDangKy { get; set; }

        [Required(ErrorMessage = "Mã học viên bắt buộc")]
        [Display(Name = "Học viên")]
        public int MaHocVien { get; set; }

        [Required(ErrorMessage = "Mã lớp học bắt buộc")]
        [Display(Name = "Lớp học")]
        public int MaLopHoc { get; set; }

        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Ghi chú học viên")]
        public string? GhiChu { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái đăng ký")]
        // Các trạng thái theo Mục 7.2: Chờ duyệt, Đã duyệt, Đã đóng học phí, Đang học, Hoàn thành, Không hoàn thành, Từ chối, Đã hủy
        public string TrangThai { get; set; } = "Chờ duyệt";

        [Display(Name = "Ngày giáo vụ xử lý")]
        public DateTime? NgayXuLy { get; set; }

        [StringLength(500)]
        [Display(Name = "Nhận xét của giáo vụ")]
        public string? NhanXetGiaoVu { get; set; }

        // Ghi nhận học phí trực tiếp theo Mục 8.4 (không cần bảng ThanhToan riêng)
        [Display(Name = "Ngày đóng học phí")]
        public DateTime? NgayDongHocPhi { get; set; }

        [Display(Name = "Số tiền đã đóng (VNĐ)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTienDaDong { get; set; } = 0;

        // Navigation properties
        [ForeignKey("MaHocVien")]
        public virtual HocVien? HocVien { get; set; }

        [ForeignKey("MaLopHoc")]
        public virtual LopHoc? LopHoc { get; set; }

        public virtual KetQuaHocTap? KetQuaHocTap { get; set; }
    }
}
