using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học & Giảng viên
    // GiangVien - POCO class (không phải DB entity)
    // Lưu ý: Entity GiangVien không có bảng DB riêng trong phiên bản này.
    // TenGiangVien được lưu trực tiếp vào cột TenGiangVien của bảng LopHoc.
    // Danh sách giảng viên được quản lý in-memory trong GiangVienRepository.
    // ========================================================
    public class GiangVien
    {
        [Display(Name = "Mã giảng viên")]
        public int MaGiangVien { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ tên giảng viên không được để trống")]
        [StringLength(200)]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Chuyên môn")]
        [StringLength(200)]
        public string? ChuyenMon { get; set; }

        [Display(Name = "Email")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [StringLength(20)]
        public string? SoDienThoai { get; set; }
    }
}
