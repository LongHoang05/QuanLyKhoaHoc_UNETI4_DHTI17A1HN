// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - Entity ChuongTrinhDaoTao

using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities
{
    public class ChuongTrinhDaoTao
    {
        [Key]
        public int MaChuongTrinh { get; set; }

        [Required(ErrorMessage = "Tên chương trình là bắt buộc")]
        [MaxLength(150)]
        [Display(Name = "Tên chương trình")]
        public string TenChuongTrinh { get; set; } = "";

        [MaxLength(2000)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [MaxLength(100)]
        [Display(Name = "Email liên hệ")]
        public string? EmailLienHe { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Hoạt động";

        // Thuộc tính điều hướng: 1 chương trình có nhiều lớp
        public ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
    }
}