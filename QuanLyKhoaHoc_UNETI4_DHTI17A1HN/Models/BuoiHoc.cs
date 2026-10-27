using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Nguyễn Tùng Dương
    // Mã sinh viên: 23103100003
    // Phụ trách: Module 5 - Lịch học, Kết quả học tập, Dashboard và Thống kê
    // ========================================================
    [Table("BuoiHoc")]
    public class BuoiHoc
    {
        [Key]
        [Display(Name = "Mã buổi học")]
        public int MaBuoiHoc { get; set; }

        [Required(ErrorMessage = "Lớp học bắt buộc")]
        [Display(Name = "Lớp học")]
        public int MaLopHoc { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu bắt buộc")]
        [Display(Name = "Thời gian bắt đầu")]
        [DataType(DataType.DateTime)]
        public DateTime ThoiGianBatDau { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc bắt buộc")]
        [Display(Name = "Thời gian kết thúc")]
        [DataType(DataType.DateTime)]
        public DateTime ThoiGianKetThuc { get; set; }

        [Required(ErrorMessage = "Phòng học không được để trống")]
        [StringLength(100)]
        [Display(Name = "Phòng học")]
        public string PhongHoc { get; set; } = string.Empty;

        [Display(Name = "Mã giảng viên")]
        public int? MaGiangVien { get; set; }

        [Required(ErrorMessage = "Tên giảng viên không được để trống")]
        [StringLength(200)]
        [Display(Name = "Giảng viên đứng lớp")]
        public string GiangVien { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Nội dung buổi học")]
        public string? NoiDung { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        // Trạng thái: "Đã lên lịch", "Đã hoàn thành", "Đã hủy"
        public string TrangThai { get; set; } = "Đã lên lịch";

        // Navigation property
        [ForeignKey("MaLopHoc")]
        public virtual LopHoc? LopHoc { get; set; }

        [ForeignKey("MaGiangVien")]
        public virtual GiangVien? GiangVienObj { get; set; }
    }
}
