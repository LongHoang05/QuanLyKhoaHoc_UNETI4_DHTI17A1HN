// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - Entity KetQuaHocTap

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities
{
    public class KetQuaHocTap
    {
        [Key]
        public int MaKetQua { get; set; }

        [ForeignKey("DangKyHoc")]
        public int MaDangKy { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        [Range(0, 10, ErrorMessage = "Điểm phải trong khoảng 0-10")]
        [Display(Name = "Điểm chuyên cần")]
        public decimal DiemChuyenCan { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        [Range(0, 10, ErrorMessage = "Điểm phải trong khoảng 0-10")]
        [Display(Name = "Điểm cuối khóa")]
        public decimal DiemCuoiKhoa { get; set; }

        // Do hệ thống tính: 20% chuyên cần + 80% cuối khóa
        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Điểm tổng kết")]
        public decimal DiemTongKet { get; set; }

        [MaxLength(30)]
        [Display(Name = "Kết quả")]
        public string? KetQua { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Nhận xét")]
        public string? NhanXet { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime NgayCapNhat { get; set; }

        // Thuộc tính điều hướng
        public DangKyHoc? DangKyHoc { get; set; }
    }
}