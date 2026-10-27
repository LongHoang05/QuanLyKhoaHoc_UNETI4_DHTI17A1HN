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

        #region Thuộc tính mở rộng phục vụ Cổng Giảng viên & Chấm điểm UNETI
        [NotMapped]
        [Display(Name = "Điểm giữa kỳ")]
        public double? DiemGiuaKy { get; set; }

        [NotMapped]
        [Display(Name = "Điểm cuối kỳ")]
        public double DiemCuoiKy
        {
            get => DiemCuoiKhoa;
            set => DiemCuoiKhoa = value;
        }

        [NotMapped]
        [Display(Name = "Điểm hệ 4")]
        public double? DiemHe4 { get; set; }

        [NotMapped]
        [Display(Name = "Điểm chữ")]
        public string? DiemChu { get; set; }

        [NotMapped]
        [Display(Name = "Xếp loại")]
        public string? XepLoai { get; set; }

        [NotMapped]
        [Display(Name = "Ghi chú")]
        public string? GhiChu
        {
            get => NhanXet;
            set => NhanXet = value;
        }

        [NotMapped]
        [Display(Name = "Ngày đánh giá")]
        public DateTime NgayDanhGia
        {
            get => NgayCapNhat;
            set => NgayCapNhat = value;
        }
        #endregion

        // Navigation property
        [ForeignKey("MaDangKy")]
        public virtual DangKyHoc? DangKyHoc { get; set; }
    }
}
