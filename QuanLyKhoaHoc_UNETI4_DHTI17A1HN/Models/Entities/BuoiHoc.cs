// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - Entity BuoiHoc

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Constants;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities
{
    public class BuoiHoc
    {
        [Key]
        public int MaBuoiHoc { get; set; }

        [ForeignKey("LopHoc")]
        [Display(Name = "Lớp học")]
        public int MaLop { get; set; }

        [Display(Name = "Thời gian bắt đầu")]
        public DateTime ThoiGianBatDau { get; set; }

        [Display(Name = "Thời gian kết thúc")]
        public DateTime ThoiGianKetThuc { get; set; }

        [Required(ErrorMessage = "Phòng học là bắt buộc")]
        [MaxLength(50)]
        [Display(Name = "Phòng học")]
        public string PhongHoc { get; set; } = "";

        [Required(ErrorMessage = "Giảng viên là bắt buộc")]
        [MaxLength(100)]
        [Display(Name = "Giảng viên")]
        public string GiangVien { get; set; } = "";

        [MaxLength(1000)]
        [Display(Name = "Nội dung")]
        public string? NoiDung { get; set; }

        [MaxLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đã lên lịch";

        // Thuộc tính điều hướng
        public LopHoc? LopHoc { get; set; }
    }
}