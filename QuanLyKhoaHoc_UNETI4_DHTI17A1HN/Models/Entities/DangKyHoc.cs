// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - Entity DangKyHoc

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities
{
    public class DangKyHoc
    {
        [Key]
        public int MaDangKy { get; set; }

        [ForeignKey("HocVien")]
        public int MaHocVien { get; set; }

        [ForeignKey("LopHoc")]
        public int MaLop { get; set; }

        // Do hệ thống gán, học viên không được tự nhập
        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Chờ duyệt";

        [Display(Name = "Ngày xử lý")]
        public DateTime? NgayXuLy { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Nhận xét giáo vụ")]
        public string? NhanXetGiaoVu { get; set; }

        [Display(Name = "Ngày đóng học phí")]
        public DateTime? NgayDongHocPhi { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Số tiền đã đóng")]
        public decimal? SoTienDaDong { get; set; }

        // Thuộc tính điều hướng
        public HocVien? HocVien { get; set; }
        public LopHoc? LopHoc { get; set; }
        public KetQuaHocTap? KetQuaHocTap { get; set; }
    }
}