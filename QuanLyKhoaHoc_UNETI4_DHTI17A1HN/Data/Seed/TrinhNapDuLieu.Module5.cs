// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - dữ liệu mẫu Module 5 (buổi học, kết quả học tập)
// Chủ sở hữu tiếp theo: Nguyễn Tùng Dương (23103100003) rà soát và bổ sung phần này.

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Constants;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    public static partial class TrinhNapDuLieu
    {
        private static void NapModule5(KhoaHocDbContext db)
        {
            var hom = DateTime.Today;
            var lop = db.LopHocs.ToDictionary(l => l.TenLop);

            if (!db.BuoiHocs.Any())
            {
                var dsBuoi = new List<BuoiHoc>();

                // ngay: số ngày so với hôm nay. gio: giờ bắt đầu, mỗi buổi kéo dài 2 giờ.
                void ThemBuoi(string tenLop, string phong, int ngay, int gio, string trangThai, string noiDung)
                {
                    var l = lop[tenLop];
                    var batDau = hom.AddDays(ngay).AddHours(gio);
                    dsBuoi.Add(new BuoiHoc
                    {
                        MaLop = l.MaLop,
                        PhongHoc = phong,
                        GiangVien = l.GiangVien,
                        ThoiGianBatDau = batDau,
                        ThoiGianKetThuc = batDau.AddHours(2),
                        NoiDung = noiDung,
                        TrangThai = trangThai,
                        GhiChu = trangThai == TrangThaiBuoiHoc.DaHuy ? "Giảng viên bận đột xuất, sẽ xếp lịch bù" : null
                    });
                }

                // Mỗi lớp dùng một phòng riêng và một khung giờ riêng, nên không trùng lớp, phòng hay giảng viên.
                // Hai lớp của cô Lê Thu Hà (A1 và A2) khác ngày hoặc khác giờ (18h và 9h).

                // Các buổi sắp tới của lớp đã có người đóng học phí nhưng chưa khai giảng
                for (int i = 0; i < 3; i++)
                {
                    ThemBuoi(LopNetCoBan, "P101", 15 + i * 3, 18, TrangThaiBuoiHoc.DaLenLich, $"Buổi {i + 1}: Cú pháp C# và kiểu dữ liệu");
                    ThemBuoi(LopAspNetMvc, "P102", 20 + i * 3, 18, TrangThaiBuoiHoc.DaLenLich, $"Buổi {i + 1}: Mô hình MVC và Routing");
                    ThemBuoi(LopAnhA1, "P201", 10 + i * 2, 18, TrangThaiBuoiHoc.DaLenLich, $"Buổi {i + 1}: Chào hỏi và giới thiệu bản thân");
                    ThemBuoi(LopAnhA2, "P202", 12 + i * 2, 9, TrangThaiBuoiHoc.DaLenLich, $"Buổi {i + 1}: Hội thoại hằng ngày");
                }

                // IELTS Foundation: đã khai giảng, có buổi đã học, sắp học và một buổi bị hủy
                ThemBuoi(LopIelts, "P301", -3, 18, TrangThaiBuoiHoc.DaHoanThanh, "Buổi 1: Giới thiệu bốn kỹ năng IELTS");
                ThemBuoi(LopIelts, "P301", -1, 18, TrangThaiBuoiHoc.DaHoanThanh, "Buổi 2: Listening phần 1");
                ThemBuoi(LopIelts, "P301", 1, 18, TrangThaiBuoiHoc.DaLenLich, "Buổi 3: Listening phần 2");
                ThemBuoi(LopIelts, "P301", 3, 18, TrangThaiBuoiHoc.DaLenLich, "Buổi 4: Reading phần 1");
                ThemBuoi(LopIelts, "P301", 5, 18, TrangThaiBuoiHoc.DaLenLich, "Buổi 5: Reading phần 2");
                ThemBuoi(LopIelts, "P301", 7, 18, TrangThaiBuoiHoc.DaHuy, "Buổi 6: Writing task 1");

                // Excel cho kế toán: đủ 8 buổi, tất cả đã hoàn thành (lớp đã học xong, được nhập kết quả)
                for (int i = 0; i < 8; i++)
                    ThemBuoi(LopExcelKeToan, "P302", -20 + i * 2, 18, TrangThaiBuoiHoc.DaHoanThanh, $"Buổi {i + 1}: Hàm Excel cho kế toán");

                // Thiết kế UI/UX: đủ 8 buổi, đã học xong từ lâu
                for (int i = 0; i < 8; i++)
                    ThemBuoi(LopUiUx, "P303", -30 + i * 3, 18, TrangThaiBuoiHoc.DaHoanThanh, $"Buổi {i + 1}: Thiết kế giao diện và trải nghiệm");

                db.BuoiHocs.AddRange(dsBuoi);
                db.SaveChanges();
            }

            if (!db.KetQuaHocTaps.Any())
            {
                var hocVien = db.HocViens.OrderBy(h => h.MaHocVien).ToList();
                int maLopUiUx = lop[LopUiUx].MaLop;
                var dangKyUiUx = db.DangKyHocs.Where(d => d.MaLop == maLopUiUx).ToList();

                // Khóa: số thứ tự học viên. Giá trị: (điểm chuyên cần, điểm cuối khóa).
                // Điểm tổng kết = 20% chuyên cần + 80% cuối khóa. Từ 5 trở lên là Hoàn thành.
                var diemMau = new Dictionary<int, (decimal chuyenCan, decimal cuoiKhoa)>
                {
                    [1] = (9m, 8.5m),
                    [2] = (8m, 7.5m),
                    [3] = (10m, 9m),
                    [5] = (5m, 3.5m),    // Không hoàn thành
                    [9] = (7m, 6m),
                    [10] = (4m, 4m),     // Không hoàn thành
                    [15] = (9m, 8m)
                };

                foreach (var mau in diemMau)
                {
                    int maHocVien = hocVien[mau.Key - 1].MaHocVien;
                    var dk = dangKyUiUx.First(x => x.MaHocVien == maHocVien);
                    decimal tongKet = Math.Round(mau.Value.chuyenCan * 0.2m + mau.Value.cuoiKhoa * 0.8m, 2);

                    db.KetQuaHocTaps.Add(new KetQuaHocTap
                    {
                        MaDangKy = dk.MaDangKy,
                        DiemChuyenCan = mau.Value.chuyenCan,
                        DiemCuoiKhoa = mau.Value.cuoiKhoa,
                        DiemTongKet = tongKet,
                        KetQua = dk.TrangThai,   // Hoàn thành hoặc Không hoàn thành, khớp trạng thái đăng ký
                        NhanXet = tongKet >= 5m ? "Đạt yêu cầu của khóa học" : "Chưa đạt yêu cầu, cần học lại",
                        NgayCapNhat = hom.AddDays(-8)
                    });
                }

                db.SaveChanges();
            }
        }
    }
}
