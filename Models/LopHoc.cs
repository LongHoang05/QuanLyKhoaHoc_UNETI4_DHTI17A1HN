// Họ và tên: Nguyễn Đức Linh
// Mã sinh viên: 23103100042
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    public class LopHoc
    {
        [Key]
        public int MaLop { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        [StringLength(100)]
        public string TenLop { get; set; } = string.Empty;

        [Required]
        public int MaChuongTrinh { get; set; }

        [StringLength(100)]
        public string? GiangVien { get; set; }

        [StringLength(50)]
        public string? TrinhDoDauVao { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sĩ số tối đa phải lớn hơn 0")]
        public int SiSoToiDa { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số buổi học phải lớn hơn 0")]
        public int SoBuoiHoc { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Học phí không được âm")]
        public decimal HocPhi { get; set; }

        [DataType(DataType.Date)]
        public System.DateTime NgayBatDauDangKy { get; set; }

        [DataType(DataType.Date)]
        public System.DateTime HanDangKy { get; set; }

        [DataType(DataType.Date)]
        public System.DateTime NgayKhaiGiang { get; set; }

        public string? MoTa { get; set; }
        public string? YeuCauHocVien { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = string.Empty;

        // Navigation property: 1 LopHoc - n DangKyHoc
        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
    }
}
