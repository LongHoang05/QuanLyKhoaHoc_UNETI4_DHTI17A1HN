// ========================================================
// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Phụ trách: Module 5 - Buổi học, Điểm danh, Kết quả học tập & Thống kê báo cáo
// Bài tập thực hành: Repository Pattern (Bài tập 12, 13, 14, 15 - UNETI)
// ========================================================

using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public class ThongKeRepository : IThongKeRepository
    {
        private readonly ApplicationDbContext _context;

        public ThongKeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, object>> GetTongHopKpiAsync()
        {
            var now = DateTime.Now;

            int tongCTDT = await _context.ChuongTrinhDaoTaos.CountAsync();
            int tongLop = await _context.LopHocs.CountAsync();
            int lopDangMo = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký");
            int tongHocVien = await _context.HocViens.CountAsync();
            int tongDangKy = await _context.DangKyHocs.CountAsync();
            int dangKyChoDuyet = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ duyệt");
            int dangKyDaDuyet = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học");
            int buoiHocSapToi = await _context.BuoiHocs.CountAsync(b => b.ThoiGianBatDau > now && b.TrangThai == "Đã lên lịch");
            int tongBuoiHoc = await _context.BuoiHocs.CountAsync();

            int totalCoDiem = await _context.KetQuaHocTaps.CountAsync();
            int soHoanThanh = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Hoàn thành");
            double tyLeHoanThanh = totalCoDiem > 0 ? ((double)soHoanThanh / totalCoDiem * 100) : 0;

            decimal tongDoanhThu = await _context.DangKyHocs
                .Where(d => d.NgayDongHocPhi != null)
                .SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0m;

            int tongChoHoc = await _context.LopHocs.SumAsync(l => (int?)l.SiSoToiDa) ?? 0;

            return new Dictionary<string, object>
            {
                { "TongSoCTDT", tongCTDT },
                { "TongSoLop", tongLop },
                { "LopDangMo", lopDangMo },
                { "TongSoHocVien", tongHocVien },
                { "TongSoDangKy", tongDangKy },
                { "DangKyChoDuyet", dangKyChoDuyet },
                { "DangKyDaDuyet", dangKyDaDuyet },
                { "BuoiHocSapToi", buoiHocSapToi },
                { "TongBuoiHoc", tongBuoiHoc },
                { "TyLeHoanThanh", tyLeHoanThanh },
                { "HocVienHoanThanh", soHoanThanh },
                { "TongDoanhThu", (double)tongDoanhThu },
                { "TongChoHoc", tongChoHoc },
                { "ChoDaDangKy", dangKyDaDuyet }
            };
        }

        public async Task<IEnumerable<dynamic>> GetLopTheoCTDTAsync()
        {
            return await _context.LopHocs
                .Where(l => l.ChuongTrinhDaoTao != null)
                .GroupBy(l => l.ChuongTrinhDaoTao!.TenChuongTrinh)
                .Select(g => (dynamic)new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetDKTheoLopAsync()
        {
            return await _context.DangKyHocs
                .Where(d => d.LopHoc != null)
                .GroupBy(d => d.LopHoc!.TenLop)
                .Select(g => (dynamic)new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetDKTheoTrangThaiAsync()
        {
            return await _context.DangKyHocs
                .GroupBy(d => d.TrangThai)
                .Select(g => (dynamic)new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<dynamic?> GetLopNhieuDKNhatAsync()
        {
            return await _context.DangKyHocs
                .Where(d => d.LopHoc != null)
                .GroupBy(d => d.LopHoc!.TenLop)
                .OrderByDescending(g => g.Count())
                .Select(g => (dynamic)new { Ten = g.Key, SoLuong = g.Count() })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ThongKeDoanhThuVM>> GetDoanhThuTheoThangAsync()
        {
            return await _context.DangKyHocs
                .Where(d => d.NgayDongHocPhi != null)
                .GroupBy(d => new { d.NgayDongHocPhi!.Value.Year, d.NgayDongHocPhi!.Value.Month })
                .Select(g => new ThongKeDoanhThuVM
                {
                    Thang = g.Key.Month + "/" + g.Key.Year,
                    TongDoanhThu = g.Sum(d => d.SoTienDaDong)
                })
                .OrderBy(r => r.Thang)
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetDoanhThuTheoCTDTAsync()
        {
            return await _context.DangKyHocs
                .Where(d => d.NgayDongHocPhi != null && d.LopHoc != null && d.LopHoc.ChuongTrinhDaoTao != null)
                .GroupBy(d => d.LopHoc!.ChuongTrinhDaoTao!.TenChuongTrinh)
                .Select(g => (dynamic)new { Ten = g.Key, DoanhThu = g.Sum(d => d.SoTienDaDong) })
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetDKTheoThangAsync()
        {
            return await _context.DangKyHocs
                .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                .Select(g => (dynamic)new { Thang = g.Key.Month + "/" + g.Key.Year, SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetBuoiHocTheoThangAsync()
        {
            return await _context.BuoiHocs
                .GroupBy(b => new { b.ThoiGianBatDau.Year, b.ThoiGianBatDau.Month })
                .Select(g => (dynamic)new { Thang = g.Key.Month + "/" + g.Key.Year, SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetDiemTBTheoLopAsync()
        {
            return await _context.KetQuaHocTaps
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.LopHoc != null)
                .GroupBy(k => k.DangKyHoc!.LopHoc!.TenLop)
                .Select(g => (dynamic)new { Ten = g.Key, DiemTB = g.Average(k => k.DiemTongKet) })
                .ToListAsync();
        }

        public async Task<double> GetTyLeHoanThanhKhoaHocAsync()
        {
            int total = await _context.KetQuaHocTaps.CountAsync();
            if (total == 0) return 0;

            int passed = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Hoàn thành");
            return (double)passed / total * 100;
        }

        public async Task<List<TyLeLapDayVM>> GetTyLeLapDayAsync()
        {
            return await _context.LopHocs
                .Select(l => new TyLeLapDayVM
                {
                    TenLop = l.TenLop,
                    SiSoToiDa = l.SiSoToiDa,
                    SiSoHienTai = _context.DangKyHocs.Count(dk => dk.MaLopHoc == l.MaLopHoc && 
                                  (dk.TrangThai == "Đã duyệt" || dk.TrangThai == "Đã đóng học phí" || dk.TrangThai == "Đang học")),
                    TyLe = l.SiSoToiDa > 0 
                        ? (double)_context.DangKyHocs.Count(dk => dk.MaLopHoc == l.MaLopHoc && 
                          (dk.TrangThai == "Đã duyệt" || dk.TrangThai == "Đã đóng học phí" || dk.TrangThai == "Đang học")) / l.SiSoToiDa * 100 
                        : 0
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetTopGiangVienAsync()
        {
            var lopList = await _context.LopHocs.ToListAsync();
            var dkList = await _context.DangKyHocs.ToListAsync();
            var kqList = await _context.KetQuaHocTaps.Include(k => k.DangKyHoc).ToListAsync();

            return lopList
                .Where(l => !string.IsNullOrEmpty(l.TenGiangVien))
                .GroupBy(l => l.TenGiangVien!)
                .Select(g => {
                    var maLops = g.Select(x => x.MaLopHoc).ToList();
                    var svCount = dkList.Count(d => maLops.Contains(d.MaLopHoc) && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));
                    var scores = kqList.Where(k => k.DangKyHoc != null && maLops.Contains(k.DangKyHoc.MaLopHoc)).Select(k => k.DiemTongKet).ToList();
                    return (dynamic)new {
                        GiangVien = g.Key,
                        SoLop = g.Count(),
                        MonHoc = string.Join(", ", g.Select(x => x.TenLop).Take(2)),
                        SoSV = svCount,
                        DiemTB = scores.Any() ? Math.Round(scores.Average(), 1) : 0,
                        DanhGia = scores.Any() ? Math.Round(Math.Min(5.0, 3.5 + (scores.Average() / 10.0) * 1.5), 1) : 4.5
                    };
                })
                .OrderByDescending(x => x.SoSV)
                .Take(5)
                .ToList();
        }
    }
}
