using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models
{
    // ========================================================
    // Họ và tên: Nguyễn Tùng Dương
    // Mã sinh viên: 23103100003
    // Phụ trách: Module 5 - Lịch học, Kết quả học tập, Dashboard và Thống kê
    // ========================================================
    [Table("KetQuaHocTap")]
    public class KetQuaHocTap
    {
        [Key]
        [Display(Name = "Mã kết quả")]
        public int MaKetQua { get; set; }

        [Required(ErrorMessage = "Mã đăng ký bắt buộc")]
        [Display(Name = "Đăng ký học")]
        public int MaDangKy { get; set; }

        [Required(ErrorMessage = "Điểm quá trình (chuyên cần) từ 0 đến 10")]
        [Range(0, 10, ErrorMessage = "Điểm quá trình trong thang điểm 0 - 10")]
        [Display(Name = "Điểm quá trình (40%)")]
        public double DiemChuyenCan { get; set; } = 0;

        [Required(ErrorMessage = "Điểm thi cuối kỳ từ 0 đến 10")]
        [Range(0, 10, ErrorMessage = "Điểm thi cuối kỳ trong thang điểm 0 - 10")]
        [Display(Name = "Điểm thi cuối kỳ (60%)")]
        public double DiemCuoiKhoa { get; set; } = 0;

        // Công thức: DiemTongKet = 40% * DiemChuyenCan + 60% * DiemCuoiKhoa
        [Display(Name = "Điểm tổng kết")]
        public double DiemTongKet { get; set; } = 0;

        [Required]
        [StringLength(50)]
        [Display(Name = "Kết quả")]
        // Kết quả: "Hoàn thành", "Không hoàn thành"
        public string KetQua { get; set; } = "Chưa xét";

        [StringLength(500)]
        [Display(Name = "Nhận xét")]
        public string? NhanXet { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        #region Thuộc tính mở rộng phục vụ Cổng Giảng viên & Chấm điểm UNETI (40% Quá trình + 60% Cuối kỳ)
        private double? _diemGiuaKy;
        [NotMapped]
        [Display(Name = "Điểm quá trình")]
        public double? DiemGiuaKy
        {
            get => _diemGiuaKy ?? DiemChuyenCan;
            set
            {
                _diemGiuaKy = value;
                if (value.HasValue) DiemChuyenCan = value.Value;
            }
        }

        [NotMapped]
        [Display(Name = "Điểm cuối kỳ")]
        public double DiemCuoiKy
        {
            get => DiemCuoiKhoa;
            set => DiemCuoiKhoa = value;
        }

        private double? _diemHe4;
        [NotMapped]
        [Display(Name = "Điểm hệ 4")]
        public double? DiemHe4
        {
            get
            {
                if (_diemHe4.HasValue) return _diemHe4;
                if (DiemTongKet <= 0 && KetQua == "Chưa xét") return null;
                if (DiemTongKet >= 8.5) return 4.0;
                if (DiemTongKet >= 8.0) return 3.5;
                if (DiemTongKet >= 7.0) return 3.0;
                if (DiemTongKet >= 6.5) return 2.5;
                if (DiemTongKet >= 5.5) return 2.0;
                if (DiemTongKet >= 5.0) return 1.5;
                if (DiemTongKet >= 4.0) return 1.0;
                return 0.0;
            }
            set => _diemHe4 = value;
        }

        private string? _diemChu;
        [NotMapped]
        [Display(Name = "Điểm chữ")]
        public string? DiemChu
        {
            get
            {
                if (!string.IsNullOrEmpty(_diemChu)) return _diemChu;
                if (DiemTongKet <= 0 && KetQua == "Chưa xét") return null;
                if (DiemTongKet >= 8.5) return "A";
                if (DiemTongKet >= 8.0) return "B+";
                if (DiemTongKet >= 7.0) return "B";
                if (DiemTongKet >= 6.5) return "C+";
                if (DiemTongKet >= 5.5) return "C";
                if (DiemTongKet >= 5.0) return "D+";
                if (DiemTongKet >= 4.0) return "D";
                return "F";
            }
            set => _diemChu = value;
        }

        private string? _xepLoai;
        [NotMapped]
        [Display(Name = "Xếp loại")]
        public string? XepLoai
        {
            get
            {
                if (!string.IsNullOrEmpty(_xepLoai)) return _xepLoai;
                if (DiemTongKet <= 0 && KetQua == "Chưa xét") return null;
                if (DiemTongKet >= 8.5) return "Xuất sắc";
                if (DiemTongKet >= 8.0) return "Giỏi";
                if (DiemTongKet >= 7.0) return "Khá";
                if (DiemTongKet >= 6.5) return "Trung bình khá";
                if (DiemTongKet >= 5.5) return "Trung bình";
                if (DiemTongKet >= 5.0) return "Trung bình yếu";
                if (DiemTongKet >= 4.0) return "Yếu";
                return "Kém";
            }
            set => _xepLoai = value;
        }

        [NotMapped]
        [Display(Name = "Ghi chú")]
        public string? GhiChu
        {
            get => NhanXet;
            set => NhanXet = value;
        }

        [NotMapped]
        [Display(Name = "Ngày đánh giá")]
        public DateTime NgayDanhGia
        {
            get => NgayCapNhat;
            set => NgayCapNhat = value;
        }
        #endregion

        // Navigation property
        [ForeignKey("MaDangKy")]
        public virtual DangKyHoc? DangKyHoc { get; set; }
    }
}
