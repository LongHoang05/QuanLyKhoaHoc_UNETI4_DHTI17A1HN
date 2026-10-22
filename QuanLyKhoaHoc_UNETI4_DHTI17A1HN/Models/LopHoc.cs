using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học, Tìm kiếm, Lọc, Sắp xếp, Phân trang
    // Entity LopHoc - Chuẩn Mục 6.1 Đề tài BTL UNETI
    // ========================================================
    [Table("LopHoc")]
    public class LopHoc
    {
        [Key]
        [Display(Name = "Mã lớp")]
        public int MaLopHoc { get; set; }

        [Required(ErrorMessage = "Tên lớp không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tên lớp học")]
        public string TenLop { get; set; } = string.Empty;

        // Khóa ngoại trỏ trực tiếp đến ChuongTrinhDaoTao (Chuẩn Mục 6.1 & Mục 10)
        [Required(ErrorMessage = "Vui lòng chọn chương trình đào tạo")]
        [Display(Name = "Chương trình đào tạo")]
        public int MaChuongTrinh { get; set; }

        [StringLength(200)]
        [Display(Name = "Tên giảng viên")]
        public string? TenGiangVien { get; set; }

        /// <summary>
        /// Mã giảng viên - [NotMapped]: chỉ dùng để binding form chọn GV,
        /// không lưu vào DB. Tên GV sau đó được đồng bộ vào TenGiangVien.
        /// </summary>
        [NotMapped]
        [Display(Name = "Giảng viên phụ trách")]
        public int? MaGiangVien { get; set; }

        [StringLength(100)]
        [Display(Name = "Trình độ đầu vào")]
        public string? TrinhDoDauVao { get; set; } // Cơ bản, Trung cấp, Nâng cao

        [Required(ErrorMessage = "Sĩ số tối đa phải > 0")]
        [Range(1, 500, ErrorMessage = "Sĩ số tối đa từ 1 đến 500")]
        [Display(Name = "Sĩ số tối đa")]
        public int SiSoToiDa { get; set; } = 30;

        [Required(ErrorMessage = "Số buổi học phải > 0")]
        [Range(1, 200, ErrorMessage = "Số buổi học từ 1 đến 200")]
        [Display(Name = "Số buổi học")]
        public int SoBuoiHoc { get; set; } = 12;

        [Required(ErrorMessage = "Học phí không được để trống")]
        [Range(0, 100000000, ErrorMessage = "Học phí phải >= 0")]
        [Display(Name = "Học phí (VNĐ)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HocPhi { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu đăng ký bắt buộc")]
        [Display(Name = "Ngày mở đăng ký")]
        [DataType(DataType.Date)]
        public DateTime NgayBatDauDangKy { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Hạn đăng ký bắt buộc")]
        [Display(Name = "Hạn chót đăng ký")]
        [DataType(DataType.Date)]
        public DateTime HanDangKy { get; set; } = DateTime.Today.AddDays(15);

        [Required(ErrorMessage = "Ngày khai giảng bắt buộc")]
        [Display(Name = "Ngày khai giảng")]
        [DataType(DataType.Date)]
        public DateTime NgayKhaiGiang { get; set; } = DateTime.Today.AddDays(20);

        [StringLength(1000)]
        [Display(Name = "Mô tả lớp học")]
        public string? MoTa { get; set; }

        [StringLength(1000)]
        [Display(Name = "Yêu cầu đối với học viên")]
        public string? YeuCauHocVien { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng thái lớp")]
        public string TrangThai { get; set; } = "Đang mở đăng ký"; // Chưa mở, Đang mở đăng ký, Tạm dừng, Đã đóng đăng ký

        [StringLength(100)]
        [Display(Name = "Phòng học")]
        public string? PhongHoc { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaChuongTrinh")]
        public virtual ChuongTrinhDaoTao? ChuongTrinhDaoTao { get; set; }

        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
        public virtual ICollection<BuoiHoc> BuoiHocs { get; set; } = new List<BuoiHoc>();
    }
}
