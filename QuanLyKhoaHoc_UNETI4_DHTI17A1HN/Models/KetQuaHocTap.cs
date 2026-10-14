using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Nguyễn Tùng Dương
    // Mã sinh viên: 23103100003
    // Phụ trách: Module 5 - Lịch học, Kết quả học tập, Dashboard và Thống kê
    // ========================================================
    [Table("KetQuaHocTap")]
    public class KetQuaHocTap
    {
        [Key]
        [Display(Name = "Mã kết quả")]
        public int MaKetQua { get; set; }

        [Required(ErrorMessage = "Mã đăng ký bắt buộc")]
        [Display(Name = "Đăng ký học")]
        public int MaDangKy { get; set; }

        [Required(ErrorMessage = "Điểm chuyên cần từ 0 đến 10")]
        [Range(0, 10, ErrorMessage = "Điểm chuyên cần trong thang điểm 0 - 10")]
        [Display(Name = "Điểm chuyên cần")]
        public double DiemChuyenCan { get; set; } = 0;

        [Required(ErrorMessage = "Điểm cuối khóa từ 0 đến 10")]
        [Range(0, 10, ErrorMessage = "Điểm cuối khóa trong thang điểm 0 - 10")]
        [Display(Name = "Điểm cuối khóa")]
        public double DiemCuoiKhoa { get; set; } = 0;

        [Display(Name = "Điểm tổng kết")]
        public double DiemTongKet { get; set; } = 0; // Công thức: 0.2 * CC + 0.8 * CK

        [Required]
        [StringLength(50)]
        [Display(Name = "Kết quả")]
        // Kết quả: "Hoàn thành", "Không hoàn thành"
        public string KetQua { get; set; } = "Chưa xét";

        [StringLength(500)]
        [Display(Name = "Nhận xét")]
        public string? NhanXet { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("MaDangKy")]
        public virtual DangKyHoc? DangKyHoc { get; set; }
    }
}
