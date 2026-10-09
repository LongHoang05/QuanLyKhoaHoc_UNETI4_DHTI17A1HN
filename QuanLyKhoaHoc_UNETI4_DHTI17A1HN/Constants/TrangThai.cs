// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - hằng số vai trò và trạng thái dùng chung

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Constants
{

    public static class LoaiVaiTro
    {
        public const string Admin = "Admin";
        public const string GiaoVu = "GiaoVu";
        public const string HocVien = "HocVien";
    }

    public static class KhoaSession
    {
        public const string MaTaiKhoan = "MaTaiKhoan";
        public const string HoTen = "HoTen";
        public const string VaiTro = "VaiTro";
    }

    public static class TrangThaiChung
    {
        public const string HoatDong = "Hoạt động";
        public const string Khoa = "Khóa";
    }

    public static class TrangThaiLop
    {
        public const string ChuaMo = "Chưa mở";
        public const string DangMo = "Đang mở đăng ký";
        public const string TamDung = "Tạm dừng";
        public const string DaDong = "Đã đóng đăng ký";
        public static readonly string[] TatCa = { ChuaMo, DangMo, TamDung, DaDong };
    }

    public static class TrangThaiDangKy
    {
        public const string ChoDuyet = "Chờ duyệt";
        public const string DaDuyet = "Đã duyệt";
        public const string DaDongHocPhi = "Đã đóng học phí";
        public const string DangHoc = "Đang học";
        public const string HoanThanh = "Hoàn thành";
        public const string KhongHoanThanh = "Không hoàn thành";
        public const string TuChoi = "Từ chối";
        public const string DaHuy = "Đã hủy";

        // Các trạng thái được tính vào sĩ số lớp (đề mục 8.3 và 8.6)
        public static readonly string[] TinhSiSo = { DaDuyet, DaDongHocPhi, DangHoc };
    }

    public static class TrangThaiBuoiHoc
    {
        public const string DaLenLich = "Đã lên lịch";
        public const string DaHoanThanh = "Đã hoàn thành";
        public const string DaHuy = "Đã hủy";
    }
}
