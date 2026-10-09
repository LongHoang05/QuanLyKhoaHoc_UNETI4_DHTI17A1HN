// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - dữ liệu mẫu Module 4 (đăng ký học đủ 8 trạng thái)

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Constants;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    public static partial class TrinhNapDuLieu
    {
        private static void NapModule4(KhoaHocDbContext db)
        {
            if (db.DangKyHocs.Any()) return;

            // hocVien[0] là học viên số 1 (hocvien01) ... hocVien[29] là học viên số 30 (bị khóa, không có đăng ký).
            var hocVien = db.HocViens.OrderBy(h => h.MaHocVien).ToList();
            var lop = db.LopHocs.ToDictionary(l => l.TenLop);
            var dsDangKy = new List<DangKyHoc>();

            // tyLeDaDong chỉ dùng cho đăng ký Đã duyệt: đã đóng một phần học phí nhưng chưa đủ.
            void Them(string tenLop, int soHocVien, string trangThai, decimal tyLeDaDong = 0m)
            {
                var l = lop[tenLop];
                var h = hocVien[soHocVien - 1];
                var dk = new DangKyHoc
                {
                    MaHocVien = h.MaHocVien,
                    MaLop = l.MaLop,
                    TrangThai = trangThai,
                    NgayDangKy = l.NgayBatDauDangKy.AddDays(2 + soHocVien % 3),
                    GhiChu = soHocVien % 4 == 0 ? "Mong muốn học vào buổi tối" : null
                };

                switch (trangThai)
                {
                    case TrangThaiDangKy.ChoDuyet:
                        break;

                    case TrangThaiDangKy.DaHuy:
                        dk.GhiChu = "Học viên tự hủy đăng ký";
                        break;

                    case TrangThaiDangKy.TuChoi:
                        dk.NgayXuLy = dk.NgayDangKy.AddDays(1);
                        dk.NhanXetGiaoVu = "Chưa đáp ứng yêu cầu đầu vào của lớp";
                        break;

                    default:
                        // Đã duyệt, Đã đóng học phí, Đang học, Hoàn thành, Không hoàn thành
                        var ngayXuLy = dk.NgayDangKy.AddDays(1);
                        dk.NgayXuLy = ngayXuLy;
                        dk.NhanXetGiaoVu = "Hồ sơ phù hợp với yêu cầu của lớp";
                        if (trangThai != TrangThaiDangKy.DaDuyet)
                        {
                            dk.NgayDongHocPhi = ngayXuLy.AddDays(1);
                            dk.SoTienDaDong = l.HocPhi;
                        }
                        else if (tyLeDaDong > 0m)
                        {
                            dk.NgayDongHocPhi = ngayXuLy.AddDays(1);
                            dk.SoTienDaDong = Math.Round(l.HocPhi * tyLeDaDong, 0);
                        }
                        break;
                }

                dsDangKy.Add(dk);
            }

            // .NET Cơ bản K1
            Them(LopNetCoBan, 1, TrangThaiDangKy.ChoDuyet);
            Them(LopNetCoBan, 2, TrangThaiDangKy.ChoDuyet);
            Them(LopNetCoBan, 3, TrangThaiDangKy.DaDuyet);
            Them(LopNetCoBan, 4, TrangThaiDangKy.DaDongHocPhi);
            Them(LopNetCoBan, 5, TrangThaiDangKy.TuChoi);
            Them(LopNetCoBan, 6, TrangThaiDangKy.DaHuy);

            // ASP.NET Core MVC K1 (học viên 9 đã đóng một nửa học phí)
            Them(LopAspNetMvc, 1, TrangThaiDangKy.DaDuyet);
            Them(LopAspNetMvc, 7, TrangThaiDangKy.ChoDuyet);
            Them(LopAspNetMvc, 8, TrangThaiDangKy.DaDongHocPhi);
            Them(LopAspNetMvc, 9, TrangThaiDangKy.DaDuyet, 0.5m);
            Them(LopAspNetMvc, 10, TrangThaiDangKy.ChoDuyet);

            // C# nâng cao (hết hạn đăng ký)
            Them(LopCSharpNangCao, 11, TrangThaiDangKy.DaDuyet);
            Them(LopCSharpNangCao, 12, TrangThaiDangKy.DaDongHocPhi);
            Them(LopCSharpNangCao, 13, TrangThaiDangKy.ChoDuyet);
            Them(LopCSharpNangCao, 14, TrangThaiDangKy.TuChoi);

            // Tiếng Anh giao tiếp A1
            Them(LopAnhA1, 15, TrangThaiDangKy.ChoDuyet);
            Them(LopAnhA1, 16, TrangThaiDangKy.DaDuyet);
            Them(LopAnhA1, 17, TrangThaiDangKy.DaDongHocPhi);
            Them(LopAnhA1, 18, TrangThaiDangKy.ChoDuyet);
            Them(LopAnhA1, 2, TrangThaiDangKy.DaHuy);

            // Tiếng Anh giao tiếp A2: sĩ số tối đa 5 và đã có đủ 5 đăng ký tính vào sĩ số.
            // Học viên 24 đang Chờ duyệt nhưng không được duyệt thêm vì lớp đã đầy.
            Them(LopAnhA2, 19, TrangThaiDangKy.DaDuyet);
            Them(LopAnhA2, 20, TrangThaiDangKy.DaDuyet);
            Them(LopAnhA2, 21, TrangThaiDangKy.DaDongHocPhi);
            Them(LopAnhA2, 22, TrangThaiDangKy.DaDongHocPhi);
            Them(LopAnhA2, 23, TrangThaiDangKy.DaDuyet);
            Them(LopAnhA2, 24, TrangThaiDangKy.ChoDuyet);
            Them(LopAnhA2, 25, TrangThaiDangKy.DaHuy);

            // IELTS Foundation (đã khai giảng). Học viên 29 đã đóng học phí nhưng chưa chuyển sang Đang học.
            Them(LopIelts, 3, TrangThaiDangKy.DangHoc);
            Them(LopIelts, 4, TrangThaiDangKy.DangHoc);
            Them(LopIelts, 16, TrangThaiDangKy.DangHoc);
            Them(LopIelts, 17, TrangThaiDangKy.DangHoc);
            Them(LopIelts, 26, TrangThaiDangKy.DangHoc);
            Them(LopIelts, 27, TrangThaiDangKy.TuChoi);
            Them(LopIelts, 28, TrangThaiDangKy.DaDuyet);
            Them(LopIelts, 29, TrangThaiDangKy.DaDongHocPhi);

            // Kế toán cơ bản (học viên 20 đã đóng 30% học phí)
            Them(LopKeToanCoBan, 12, TrangThaiDangKy.ChoDuyet);
            Them(LopKeToanCoBan, 20, TrangThaiDangKy.DaDuyet, 0.3m);
            Them(LopKeToanCoBan, 29, TrangThaiDangKy.DaDongHocPhi);
            Them(LopKeToanCoBan, 5, TrangThaiDangKy.ChoDuyet);

            // Kế toán thuế (lớp đang tạm dừng)
            Them(LopKeToanThue, 13, TrangThaiDangKy.DaDuyet);
            Them(LopKeToanThue, 14, TrangThaiDangKy.ChoDuyet);

            // Excel cho kế toán: lớp đã học xong, 5 học viên còn Đang học để thử nhập kết quả cuối cùng.
            Them(LopExcelKeToan, 11, TrangThaiDangKy.DangHoc);
            Them(LopExcelKeToan, 12, TrangThaiDangKy.DangHoc);
            Them(LopExcelKeToan, 21, TrangThaiDangKy.DangHoc);
            Them(LopExcelKeToan, 22, TrangThaiDangKy.DangHoc);
            Them(LopExcelKeToan, 6, TrangThaiDangKy.DangHoc);

            // Photoshop cơ bản
            Them(LopPhotoshop, 7, TrangThaiDangKy.ChoDuyet);
            Them(LopPhotoshop, 8, TrangThaiDangKy.DaDuyet);
            Them(LopPhotoshop, 18, TrangThaiDangKy.DaDongHocPhi);
            Them(LopPhotoshop, 23, TrangThaiDangKy.ChoDuyet);
            Them(LopPhotoshop, 27, TrangThaiDangKy.DaHuy);

            // Thiết kế UI/UX: lớp đã kết thúc, kết quả được nạp ở Module 5.
            Them(LopUiUx, 1, TrangThaiDangKy.HoanThanh);
            Them(LopUiUx, 2, TrangThaiDangKy.HoanThanh);
            Them(LopUiUx, 3, TrangThaiDangKy.HoanThanh);
            Them(LopUiUx, 5, TrangThaiDangKy.KhongHoanThanh);
            Them(LopUiUx, 9, TrangThaiDangKy.HoanThanh);
            Them(LopUiUx, 10, TrangThaiDangKy.KhongHoanThanh);
            Them(LopUiUx, 15, TrangThaiDangKy.HoanThanh);

            // Web Frontend
            Them(LopFrontend, 24, TrangThaiDangKy.ChoDuyet);
            Them(LopFrontend, 25, TrangThaiDangKy.ChoDuyet);
            Them(LopFrontend, 26, TrangThaiDangKy.DaDuyet);
            Them(LopFrontend, 28, TrangThaiDangKy.ChoDuyet);

            // Tin học văn phòng MOS
            Them(LopMos, 4, TrangThaiDangKy.ChoDuyet);
            Them(LopMos, 19, TrangThaiDangKy.DaDongHocPhi);

            db.DangKyHocs.AddRange(dsDangKy);
            db.SaveChanges();
        }
    }
}
