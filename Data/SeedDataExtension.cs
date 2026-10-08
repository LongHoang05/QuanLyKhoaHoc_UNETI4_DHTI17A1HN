using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data
{
    public static class SeedDataExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var taiKhoans = new List<TaiKhoan>();
            taiKhoans.Add(new TaiKhoan { MaTaiKhoan = 1, TenDangNhap = "admin1", MatKhau = "123456", HoTen = "Admin 1", VaiTro = "Admin", TrangThai = "Đang hoạt động", Email = "admin1@uneti.edu.vn" });
            taiKhoans.Add(new TaiKhoan { MaTaiKhoan = 2, TenDangNhap = "admin2", MatKhau = "123456", HoTen = "Admin 2", VaiTro = "Admin", TrangThai = "Đang hoạt động", Email = "admin2@uneti.edu.vn" });
            for(int i=1; i<=3; i++) taiKhoans.Add(new TaiKhoan { MaTaiKhoan = 2+i, TenDangNhap = $"giaovu{i}", MatKhau = "123456", HoTen = $"Giáo vụ {i}", VaiTro = "Giáo vụ", TrangThai = "Đang hoạt động", Email = $"giaovu{i}@uneti.edu.vn" });
            for(int i=1; i<=30; i++) taiKhoans.Add(new TaiKhoan { MaTaiKhoan = 5+i, TenDangNhap = $"hocvien{i}", MatKhau = "123456", HoTen = $"Học viên {i}", VaiTro = "Học viên", TrangThai = "Đang hoạt động", Email = $"hocvien{i}@gmail.com" });
            modelBuilder.Entity<TaiKhoan>().HasData(taiKhoans);

            var hocViens = new List<HocVien>();
            for(int i=1; i<=30; i++) hocViens.Add(new HocVien { MaHocVien = i, MaTaiKhoan = 5+i, HoTen = $"Học viên {i}", NgaySinh = new DateTime(2000, 1, 1).AddDays(i*30), GioiTinh = i%2==0 ? "Nam" : "Nữ", SoDienThoai = "0987654" + i.ToString("D3"), Email = $"hocvien{i}@gmail.com", DiaChi = "Hà Nội", TrinhDoHocVan = "Đại học", NgheNghiep = "Sinh viên", TrangThai = "Đang học" });
            modelBuilder.Entity<HocVien>().HasData(hocViens);

            var ctdts = new List<ChuongTrinhDaoTao>();
            string[] tenCtdts = { "Lập trình Web ASP.NET Core", "Lập trình ReactJS", "Khoa học dữ liệu với Python", "Tiếng Anh Giao Tiếp", "Lập trình Mobile Flutter" };
            for(int i=1; i<=5; i++) ctdts.Add(new ChuongTrinhDaoTao { MaChuongTrinh = i, TenChuongTrinh = tenCtdts[i-1], MoTa = "Chương trình " + tenCtdts[i-1], EmailLienHe = "contact@uneti.edu.vn", TrangThai = "Đang hoạt động" });
            modelBuilder.Entity<ChuongTrinhDaoTao>().HasData(ctdts);

            var lopHocs = new List<LopHoc>();
            string[] trangThaiLop = { "Đang mở đăng ký", "Đã đóng đăng ký", "Đang học", "Đã kết thúc", "Hết hạn đăng ký" };
            for(int i=1; i<=15; i++) lopHocs.Add(new LopHoc { 
                MaLop = i, MaChuongTrinh = ((i-1)%5)+1, TenLop = $"Lớp {tenCtdts[((i-1)%5)]} Khóa {i}", 
                GiangVien = $"Giảng viên {i%3+1}", TrinhDoDauVao = "Cơ bản", 
                SiSoToiDa = 30, SoBuoiHoc = 15, HocPhi = 5000000, 
                NgayBatDauDangKy = new DateTime(2023, 1, 1).AddMonths(i%12), 
                HanDangKy = new DateTime(2023, 1, 20).AddMonths(i%12), 
                NgayKhaiGiang = new DateTime(2023, 2, 1).AddMonths(i%12), 
                MoTa = "Mô tả lớp học", YeuCauHocVien = "Không", 
                TrangThai = trangThaiLop[(i-1)%5] 
            });
            modelBuilder.Entity<LopHoc>().HasData(lopHocs);

            var dangKyHocs = new List<DangKyHoc>();
            string[] trangThaiDK = { "Chờ duyệt", "Đã duyệt", "Đã đóng học phí", "Đang học", "Đã hủy" };
            for(int i=1; i<=45; i++) dangKyHocs.Add(new DangKyHoc { 
                MaDangKy = i, MaHocVien = ((i-1)%30)+1, MaLop = ((i-1)%15)+1, 
                NgayDangKy = new DateTime(2023, 1, 15).AddDays(i), 
                GhiChu = "", TrangThai = trangThaiDK[(i-1)%5], 
                NgayXuLy = new DateTime(2023, 1, 16).AddDays(i), 
                NhanXetGiaoVu = "Đã kiểm tra", 
                NgayDongHocPhi = trangThaiDK[(i-1)%5] == "Đã đóng học phí" || trangThaiDK[(i-1)%5] == "Đang học" ? new DateTime(2023, 1, 17).AddDays(i) : null, 
                SoTienDaDong = trangThaiDK[(i-1)%5] == "Đã đóng học phí" || trangThaiDK[(i-1)%5] == "Đang học" ? 5000000 : 0 
            });
            modelBuilder.Entity<DangKyHoc>().HasData(dangKyHocs);

            var buoiHocs = new List<BuoiHoc>();
            for(int i=1; i<=30; i++) buoiHocs.Add(new BuoiHoc { 
                MaBuoiHoc = i, MaLop = ((i-1)%15)+1, 
                ThoiGianBatDau = new DateTime(2023, 2, 1).AddDays(i).AddHours(18), 
                ThoiGianKetThuc = new DateTime(2023, 2, 1).AddDays(i).AddHours(21), 
                PhongHoc = $"Phòng {100 + i%5}", GiangVien = $"Giảng viên {i%3+1}", 
                NoiDung = $"Buổi học {i}", GhiChu = "", 
                TrangThai = i % 2 == 0 ? "Đã hoàn thành" : "Đã lên lịch" 
            });
            modelBuilder.Entity<BuoiHoc>().HasData(buoiHocs);

            var ketQuaHocTaps = new List<KetQuaHocTap>();
            int kqId = 1;
            for(int i=1; i<=45; i++) {
                if(trangThaiDK[(i-1)%5] == "Đang học" || trangThaiDK[(i-1)%5] == "Đã đóng học phí") {
                    ketQuaHocTaps.Add(new KetQuaHocTap {
                        MaKetQua = kqId++, MaDangKy = i, DiemChuyenCan = 8 + (i%3), DiemCuoiKhoa = 7 + (i%4), 
                        DiemTongKet = (8 + (i%3))*0.3 + (7 + (i%4))*0.7, 
                        KetQua = ((8 + (i%3))*0.3 + (7 + (i%4))*0.7) >= 5 ? "Hoàn thành" : "Không hoàn thành",
                        NhanXet = "Tốt", NgayCapNhat = new DateTime(2023, 5, 1).AddDays(i)
                    });
                }
            }
            modelBuilder.Entity<KetQuaHocTap>().HasData(ketQuaHocTaps);
        }
    }
}
