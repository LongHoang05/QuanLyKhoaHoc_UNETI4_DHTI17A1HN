// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - Entity LopHoc

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities
{
    public class LopHoc
    {
        [Key]
        public int MaLop { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        [MaxLength(150)]
        [Display(Name = "Tên lớp")]
        public string TenLop { get; set; } = "";

        [ForeignKey("ChuongTrinh")]
        [Display(Name = "Chương trình đào tạo")]
        public int MaChuongTrinh { get; set; }

        [Required(ErrorMessage = "Giảng viên là bắt buộc")]
        [MaxLength(100)]
        [Display(Name = "Giảng viên")]
        public string GiangVien { get; set; } = "";

        [MaxLength(100)]
        [Display(Name = "Trình độ đầu vào")]
        public string? TrinhDoDauVao { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sĩ số tối đa phải lớn hơn 0")]
        [Display(Name = "Sĩ số tối đa")]
        public int SiSoToiDa { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số buổi học phải lớn hơn 0")]
        [Display(Name = "Số buổi học")]
        public int SoBuoiHoc { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Range(0, double.MaxValue, ErrorMessage = "Học phí phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Học phí")]
        public decimal HocPhi { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu đăng ký")]
        public DateTime NgayBatDauDangKy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hạn đăng ký")]
        public DateTime HanDangKy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày khai giảng")]
        public DateTime NgayKhaiGiang { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Yêu cầu học viên")]
        public string? YeuCauHocVien { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Chưa mở";

        // Thuộc tính điều hướng
        public ChuongTrinhDaoTao? ChuongTrinh { get; set; }
        public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
        public ICollection<BuoiHoc> BuoiHocs { get; set; } = new List<BuoiHoc>();
    }
}