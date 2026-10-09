// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - dữ liệu mẫu Module 1 (tài khoản, chương trình đào tạo)
// Chủ sở hữu tiếp theo: Vũ Đình Trường (23103100026) rà soát và bổ sung phần này.

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Constants;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    public static partial class TrinhNapDuLieu
    {
        // 30 học viên mẫu, dùng chung cho Module 1 (tài khoản) và Module 3 (hồ sơ).
        // Người thứ 30 là tài khoản bị khóa để thử đăng nhập.
        private static readonly string[] TenHocVienMau =
        {
            "Nguyễn Văn An", "Trần Thị Bình", "Lê Hoàng Cường", "Phạm Thị Dung", "Hoàng Văn Em",
            "Vũ Thị Phương", "Đặng Minh Quân", "Bùi Thị Hà", "Đỗ Văn Hải", "Ngô Thị Lan",
            "Dương Văn Khánh", "Lý Thị Mai", "Phan Văn Nam", "Trịnh Thị Oanh", "Đinh Văn Phúc",
            "Mai Thị Quyên", "Tạ Văn Sơn", "Hồ Thị Thảo", "Lương Văn Tuấn", "Chu Thị Uyên",
            "Kiều Văn Việt", "Cao Thị Xuân", "Lâm Văn Yên", "Tô Thị Ánh", "Quách Văn Bảo",
            "Từ Thị Chi", "Giang Văn Đạt", "Doãn Thị Giang", "Thái Văn Hiếu", "Khuất Thị Ngọc"
        };

        private static TaiKhoan TaoTaiKhoan(string tenDangNhap, string hoTen, string vaiTro,
            string trangThai = TrangThaiChung.HoatDong)
        {
            return new TaiKhoan
            {
                TenDangNhap = tenDangNhap,
                MatKhau = MatKhauMau,
                HoTen = hoTen,
                Email = $"{tenDangNhap}@example.com",
                VaiTro = vaiTro,
                TrangThai = trangThai
            };
        }

        private static void NapModule1(KhoaHocDbContext db)
        {
            if (!db.TaiKhoans.Any())
            {
                var dsTaiKhoan = new List<TaiKhoan>
                {
                    TaoTaiKhoan("admin", "Quản trị viên", LoaiVaiTro.Admin),
                    TaoTaiKhoan("admin2", "Quản trị viên 2", LoaiVaiTro.Admin),
                    TaoTaiKhoan("giaovu1", "Nguyễn Thu Trang", LoaiVaiTro.GiaoVu),
                    TaoTaiKhoan("giaovu2", "Trần Quang Huy", LoaiVaiTro.GiaoVu),
                    TaoTaiKhoan("giaovu3", "Lê Minh Châu", LoaiVaiTro.GiaoVu)
                };

                for (int i = 0; i < TenHocVienMau.Length; i++)
                {
                    int so = i + 1;
                    bool biKhoa = so == TenHocVienMau.Length;   // hocvien30 bị khóa
                    dsTaiKhoan.Add(TaoTaiKhoan(
                        $"hocvien{so:D2}",
                        TenHocVienMau[i],
                        LoaiVaiTro.HocVien,
                        biKhoa ? TrangThaiChung.Khoa : TrangThaiChung.HoatDong));
                }

                db.TaiKhoans.AddRange(dsTaiKhoan);
                db.SaveChanges();
            }

            if (!db.ChuongTrinhDaoTaos.Any())
            {
                // Thứ tự quan trọng: Module 2 lấy chương trình theo vị trí 0..4.
                db.ChuongTrinhDaoTaos.AddRange(
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "Lập trình .NET",
                        MoTa = "Từ C# cơ bản đến ASP.NET Core MVC và Entity Framework Core.",
                        EmailLienHe = "laptrinh@trungtam.edu.vn",
                        TrangThai = TrangThaiChung.HoatDong
                    },
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "Tiếng Anh giao tiếp",
                        MoTa = "Các khóa tiếng Anh giao tiếp và luyện thi IELTS.",
                        EmailLienHe = "tienganh@trungtam.edu.vn",
                        TrangThai = TrangThaiChung.HoatDong
                    },
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "Kế toán doanh nghiệp",
                        MoTa = "Kế toán căn bản, kế toán thuế, Excel và tin học văn phòng cho kế toán.",
                        EmailLienHe = "ketoan@trungtam.edu.vn",
                        TrangThai = TrangThaiChung.HoatDong
                    },
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "Thiết kế đồ họa",
                        MoTa = "Photoshop, Illustrator và thiết kế giao diện UI/UX.",
                        EmailLienHe = "dohoa@trungtam.edu.vn",
                        TrangThai = TrangThaiChung.HoatDong
                    },
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "Kỹ năng mềm",
                        MoTa = "Chương trình tạm ngưng, chưa có lớp. Dùng để thử xóa và khóa chương trình.",
                        EmailLienHe = "kynangmem@trungtam.edu.vn",
                        TrangThai = TrangThaiChung.Khoa
                    });
                db.SaveChanges();
            }
        }
    }
}
