using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Bổ sung theo yêu cầu giảng viên hướng dẫn:
    // Thực thể Quản lý Giảng viên & Phân quyền Giảng dạy
    // Hỗ trợ Role Giảng viên: Xem lịch dạy & Cho điểm học viên
    // ========================================================
    [Table("GiangVien")]
    public class GiangVien
    {
        [Key]
        [Display(Name = "Mã giảng viên")]
        public int MaGiangVien { get; set; }

        [Display(Name = "Tài khoản liên kết")]
        public int? MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ và tên giảng viên không được để trống")]
        [StringLength(200, ErrorMessage = "Họ tên không vượt quá 200 ký tự")]
        [Display(Name = "Họ và tên giảng viên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(200)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [StringLength(100)]
        [Display(Name = "Học vị / Học hàm")]
        public string? HocVi { get; set; } = "Thạc sĩ"; // Cử nhân, Thạc sĩ, Tiến sĩ, PGS.TS, GS.TS

        [StringLength(200)]
        [Display(Name = "Chuyên ngành / Khoa")]
        public string? ChuyenNganh { get; set; } = "Công nghệ thông tin";

        [StringLength(500)]
        [Display(Name = "Giới thiệu / Tiểu sử")]
        public string? GioiThieu { get; set; }

        [Display(Name = "Trạng thái công tác")]
        public bool TrangThai { get; set; } = true; // true = Đang giảng dạy, false = Tạm nghỉ

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
        public virtual ICollection<BuoiHoc> BuoiHocs { get; set; } = new List<BuoiHoc>();
    }
}
