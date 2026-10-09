// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - dữ liệu mẫu Module 2 (15 lớp học)
// Chủ sở hữu tiếp theo: Nguyễn Đình Kiên (23103100020) rà soát và bổ sung phần này.

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Constants;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    public static partial class TrinhNapDuLieu
    {
        // Tên lớp dùng chung cho Module 4 và Module 5 để tra cứu, tránh gõ sai chuỗi.
        private const string LopNetCoBan = ".NET Cơ bản K1";
        private const string LopAspNetMvc = "ASP.NET Core MVC K1";
        private const string LopCSharpNangCao = "Lập trình C# nâng cao";
        private const string LopEfCore = "Entity Framework Core thực hành";
        private const string LopAnhA1 = "Tiếng Anh giao tiếp A1";
        private const string LopAnhA2 = "Tiếng Anh giao tiếp A2";
        private const string LopIelts = "IELTS Foundation";
        private const string LopKeToanCoBan = "Kế toán cơ bản";
        private const string LopKeToanThue = "Kế toán thuế";
        private const string LopExcelKeToan = "Excel cho kế toán";
        private const string LopPhotoshop = "Photoshop cơ bản";
        private const string LopIllustrator = "Illustrator thực hành";
        private const string LopUiUx = "Thiết kế UI/UX";
        private const string LopFrontend = "Web Frontend HTML/CSS/JS";
        private const string LopMos = "Tin học văn phòng MOS";

        private static void NapModule2(KhoaHocDbContext db)
        {
            if (db.LopHocs.Any()) return;

            var ct = db.ChuongTrinhDaoTaos.OrderBy(c => c.MaChuongTrinh).ToList();
            var hom = DateTime.Today;

            // Các mốc ngày tính theo số ngày so với hôm nay, nên dữ liệu luôn đúng trạng thái dù chạy vào ngày nào.
            LopHoc Lop(string ten, int viTriChuongTrinh, string giangVien, string trinhDo, int siSo, int soBuoi,
                decimal hocPhi, int ngayBatDauDangKy, int ngayHanDangKy, int ngayKhaiGiang, string trangThai, string moTa)
            {
                return new LopHoc
                {
                    TenLop = ten,
                    MaChuongTrinh = ct[viTriChuongTrinh].MaChuongTrinh,
                    GiangVien = giangVien,
                    TrinhDoDauVao = trinhDo,
                    SiSoToiDa = siSo,
                    SoBuoiHoc = soBuoi,
                    HocPhi = hocPhi,
                    NgayBatDauDangKy = hom.AddDays(ngayBatDauDangKy),
                    HanDangKy = hom.AddDays(ngayHanDangKy),
                    NgayKhaiGiang = hom.AddDays(ngayKhaiGiang),
                    TrangThai = trangThai,
                    MoTa = moTa,
                    YeuCauHocVien = trinhDo == "Không yêu cầu"
                        ? "Không có yêu cầu đặc biệt. Học viên mang theo thiết bị học tập cá nhân."
                        : $"Học viên cần đáp ứng: {trinhDo}."
                };
            }

            db.LopHocs.AddRange(
                // --- Chương trình Lập trình .NET ---
                Lop(LopNetCoBan, 0, "Nguyễn Văn Hùng", "Không yêu cầu", 25, 12, 3_500_000m, -20, 10, 15,
                    TrangThaiLop.DangMo, "Làm quen C# và lập trình hướng đối tượng."),
                Lop(LopAspNetMvc, 0, "Trần Minh Khoa", "Đã biết C# cơ bản", 20, 16, 5_500_000m, -15, 15, 20,
                    TrangThaiLop.DangMo, "Xây dựng ứng dụng web với ASP.NET Core MVC."),
                // Hết hạn đăng ký nhưng trạng thái vẫn là Đang mở: để thử ca lớp hết hạn.
                Lop(LopCSharpNangCao, 0, "Nguyễn Văn Hùng", "Đã học C# cơ bản", 15, 10, 4_500_000m, -30, -5, 5,
                    TrangThaiLop.DangMo, "LINQ, generic, async/await."),
                // Chưa đến ngày bắt đầu đăng ký.
                Lop(LopEfCore, 0, "Trần Minh Khoa", "Biết ASP.NET Core", 12, 8, 4_000_000m, 5, 25, 30,
                    TrangThaiLop.ChuaMo, "Entity Framework Core Code First và Migration."),

                // --- Chương trình Tiếng Anh giao tiếp ---
                Lop(LopAnhA1, 1, "Lê Thu Hà", "Không yêu cầu", 30, 20, 2_800_000m, -25, 5, 10,
                    TrangThaiLop.DangMo, "Tiếng Anh giao tiếp cho người mới bắt đầu."),
                // Sĩ số tối đa chỉ 5: sẽ đầy chỗ khi nạp đăng ký ở Module 4.
                Lop(LopAnhA2, 1, "Lê Thu Hà", "Hoàn thành A1", 5, 20, 3_000_000m, -20, 8, 12,
                    TrangThaiLop.DangMo, "Lớp nhỏ, tăng cường luyện nói."),
                // Đã đóng đăng ký và đã khai giảng.
                Lop(LopIelts, 1, "Phạm Quốc Bảo", "Tương đương A2", 20, 30, 7_500_000m, -40, -10, -3,
                    TrangThaiLop.DaDong, "Nền tảng IELTS bốn kỹ năng."),

                // --- Chương trình Kế toán doanh nghiệp ---
                Lop(LopKeToanCoBan, 2, "Đỗ Thị Lan", "Không yêu cầu", 25, 15, 3_200_000m, -18, 12, 18,
                    TrangThaiLop.DangMo, "Nguyên lý kế toán và chứng từ."),
                Lop(LopKeToanThue, 2, "Đỗ Thị Lan", "Đã học kế toán cơ bản", 20, 12, 3_800_000m, -10, 20, 25,
                    TrangThaiLop.TamDung, "Thuế GTGT, thuế TNDN. Lớp đang tạm dừng."),
                // Đã học xong toàn bộ buổi (xem Module 5): dùng để thử nhập kết quả học tập.
                Lop(LopExcelKeToan, 2, "Vũ Hải Nam", "Biết Excel cơ bản", 18, 8, 2_000_000m, -45, -25, -20,
                    TrangThaiLop.DaDong, "Hàm và báo cáo Excel dùng trong kế toán."),
                Lop(LopMos, 2, "Vũ Hải Nam", "Không yêu cầu", 30, 10, 1_800_000m, -8, 9, 14,
                    TrangThaiLop.DangMo, "Word, Excel, PowerPoint theo chuẩn MOS."),

                // --- Chương trình Thiết kế đồ họa ---
                Lop(LopPhotoshop, 3, "Hoàng Mai Anh", "Không yêu cầu", 20, 14, 3_000_000m, -12, 6, 11,
                    TrangThaiLop.DangMo, "Chỉnh sửa ảnh và thiết kế với Photoshop."),
                Lop(LopIllustrator, 3, "Hoàng Mai Anh", "Biết Photoshop", 15, 12, 3_500_000m, 3, 20, 28,
                    TrangThaiLop.ChuaMo, "Thiết kế vector với Illustrator."),
                // Đã kết thúc hoàn toàn, có kết quả Hoàn thành và Không hoàn thành.
                Lop(LopUiUx, 3, "Ngô Đức Thịnh", "Không yêu cầu", 20, 8, 6_000_000m, -60, -35, -30,
                    TrangThaiLop.DaDong, "Thiết kế giao diện và trải nghiệm người dùng."),

                // --- Bổ sung vào chương trình Lập trình .NET ---
                Lop(LopFrontend, 0, "Ngô Đức Thịnh", "Không yêu cầu", 25, 18, 4_200_000m, -5, 25, 30,
                    TrangThaiLop.DangMo, "HTML, CSS và JavaScript cho giao diện web."));

            db.SaveChanges();
        }
    }
}
