// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - bộ nạp dữ liệu mẫu (điểm vào, gọi lần lượt 5 module)

using Microsoft.EntityFrameworkCore;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    /// <summary>
    /// Nạp dữ liệu mẫu khi ứng dụng khởi động. Mỗi module có một file riêng trong thư mục Data/Seed.
    /// Quy tắc: mỗi hàm NapModuleN chỉ nạp khi bảng của nó còn trống, nên chạy nhiều lần vẫn an toàn.
    /// Thứ tự gọi bắt buộc từ 1 đến 5 vì có khóa ngoại.
    /// </summary>
    public static partial class TrinhNapDuLieu
    {
        // Mật khẩu mặc định của mọi tài khoản mẫu. Nếu Module 1 quyết định băm mật khẩu thì sửa chỗ này.
        public const string MatKhauMau = "123456";

        public static void NapDuLieu(KhoaHocDbContext db)
        {
            db.Database.Migrate();   // tự áp Migration, thành viên chỉ cần nhấn F5

            NapModule1(db);   // TaiKhoan, ChuongTrinhDaoTao
            NapModule2(db);   // LopHoc
            NapModule3(db);   // HocVien
            NapModule4(db);   // DangKyHoc
            NapModule5(db);   // BuoiHoc, KetQuaHocTap
        }
    }
}
