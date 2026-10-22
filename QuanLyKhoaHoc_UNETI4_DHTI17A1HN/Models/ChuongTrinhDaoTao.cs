using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Vũ Đình Trường
    // Mã sinh viên: 23103100026
    // Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
    // ========================================================
    [Table("ChuongTrinhDaoTao")]
    public class ChuongTrinhDaoTao
    {
        [Key]
        [Display(Name = "Mã chương trình")]
        public int MaChuongTrinh { get; set; }

        [Required(ErrorMessage = "Tên chương trình đào tạo không được để trống")]
        [StringLength(200, ErrorMessage = "Tên chương trình không vượt quá 200 ký tự")]
        [Display(Name = "Tên chương trình đào tạo")]
        public string TenChuongTrinh { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả chương trình")]
        public string? MoTa { get; set; }

        [EmailAddress(ErrorMessage = "Email liên hệ không đúng định dạng")]
        [StringLength(200)]
        [Display(Name = "Email liên hệ")]
        public string? EmailLienHe { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation property: ChuongTrinhDaoTao (1) - (n) LopHoc (Chuẩn Mục 10)
        public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
    }
}
