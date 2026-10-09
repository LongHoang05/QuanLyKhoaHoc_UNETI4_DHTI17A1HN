// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - dữ liệu mẫu Module 3 (30 hồ sơ học viên)
// Chủ sở hữu tiếp theo: Nguyễn Đức Linh (23103100042) rà soát và bổ sung phần này.

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Constants;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    public static partial class TrinhNapDuLieu
    {
        private static void NapModule3(KhoaHocDbContext db)
        {
            if (db.HocViens.Any()) return;

            // Mỗi tài khoản vai trò Học viên có một hồ sơ. Sắp theo tên đăng nhập: hocvien01 là học viên số 1.
            var taiKhoanHocVien = db.TaiKhoans
                .Where(t => t.VaiTro == LoaiVaiTro.HocVien)
                .OrderBy(t => t.TenDangNhap)
                .ToList();

            var diaChi = new[]
            {
                "Cầu Giấy, Hà Nội", "Đống Đa, Hà Nội", "Thanh Xuân, Hà Nội", "Hà Đông, Hà Nội",
                "Hoàng Mai, Hà Nội", "Long Biên, Hà Nội", "Nam Từ Liêm, Hà Nội", "Bắc Từ Liêm, Hà Nội"
            };
            var trinhDo = new[] { "THPT", "Trung cấp", "Cao đẳng", "Đại học" };
            var ngheNghiep = new[] { "Sinh viên", "Nhân viên văn phòng", "Kế toán", "Freelancer", "Kinh doanh tự do" };

            var dsHocVien = new List<HocVien>();
            for (int i = 0; i < taiKhoanHocVien.Count; i++)
            {
                var tk = taiKhoanHocVien[i];
                int so = i + 1;
                bool thieuHoSo = so == 29;                  // hồ sơ chưa khai báo đủ: thử ca đăng ký bị chặn
                bool biKhoa = so == taiKhoanHocVien.Count;  // hocvien30 bị khóa

                dsHocVien.Add(new HocVien
                {
                    MaTaiKhoan = tk.MaTaiKhoan,
                    HoTen = tk.HoTen,
                    NgaySinh = new DateTime(1990 + (so * 7) % 16, 1 + so % 12, 1 + (so * 3) % 28),
                    GioiTinh = !tk.HoTen.Contains(" Thị "),   // true = Nam, false = Nữ
                    // Số điện thoại đúng mẫu 0 + [3|5|7|8|9] + 8 chữ số
                    SoDienThoai = thieuHoSo ? null : "09" + (10_000_000 + so * 137),
                    Email = tk.Email,
                    DiaChi = thieuHoSo ? null : diaChi[i % diaChi.Length],
                    TrinhDoHocVan = thieuHoSo ? null : trinhDo[i % trinhDo.Length],
                    NgheNghiep = ngheNghiep[i % ngheNghiep.Length],
                    TrangThai = biKhoa ? TrangThaiChung.Khoa : TrangThaiChung.HoatDong
                });
            }

            db.HocViens.AddRange(dsHocVien);
            db.SaveChanges();
        }
    }
}
