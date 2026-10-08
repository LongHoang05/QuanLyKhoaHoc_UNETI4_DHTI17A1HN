// Họ và tên: Nguyễn Đức Linh
// Mã sinh viên: 23103100042
// Nội dung thực hiện: Quản lý học viên, hồ sơ cá nhân
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    public class HocVien
    {
        [Key]
        public int MaHocVien { get; set; }

        // Foreign Key to TaiKhoan (1 - 0..1)
        [Required]
        public int MaTaiKhoan { get; set; }
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [StringLength(10)]
        public string GioiTinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(200)]
        public string? DiaChi { get; set; }

        [StringLength(50)]
        public string? TrinhDoHocVan { get; set; }

        [StringLength(50)]
        public string? NgheNghiep { get; set; }

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; } = string.Empty;

        // Navigation property: 1 HocVien - n DangKyHoc
        public virtual ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
    }
}
