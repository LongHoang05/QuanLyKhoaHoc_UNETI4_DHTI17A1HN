// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Xử lý nghiệp vụ điểm số, kết quả học tập (Module 5)

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class KetQuaHocTapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KetQuaHocTapController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;
            var query = _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                .ThenInclude(d => d.HocVien)
                .Include(k => k.DangKyHoc)
                .ThenInclude(d => d.LopHoc);

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var ketQuas = await query
                .OrderByDescending(k => k.NgayCapNhat)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            // Thống kê
            ViewBag.PassCount = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Hoàn thành");
            ViewBag.FailCount = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Không hoàn thành");
            
            var allScores = await _context.KetQuaHocTaps.Select(k => k.DiemTongKet).ToListAsync();
            ViewBag.AverageScore = allScores.Any() ? allScores.Average() : 0;

            return View(ketQuas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KetQuaHocTap kq)
        {
            var dangKy = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == kq.MaDangKy);

            if (dangKy == null || dangKy.TrangThai != "Đang học")
            {
                ModelState.AddModelError("", "Chỉ được nhập điểm cho học viên Đang học.");
            }
            else
            {
                int soBuoiHoanThanh = _context.BuoiHocs
                    .Count(b => b.MaLop == dangKy.MaLop && b.TrangThai == "Đã hoàn thành");

                if (soBuoiHoanThanh < dangKy.LopHoc.SoBuoiHoc)
                {
                    ModelState.AddModelError("", "Lớp chưa hoàn thành tất cả các buổi học, chưa được nhập điểm.");
                }
            }

            if (ModelState.IsValid)
            {
                // Tự động tính điểm tổng kết (VD: 30% CC, 70% CK)
                kq.DiemTongKet = (kq.DiemChuyenCan * 0.3) + (kq.DiemCuoiKhoa * 0.7);
                kq.NgayCapNhat = DateTime.Now;

                _context.Add(kq);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(kq);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(int maKetQua, double diemChuyenCan, double diemCuoiKhoa, string nhanXet, string ketQua)
        {
            var kq = await _context.KetQuaHocTaps.FindAsync(maKetQua);
            if (kq != null)
            {
                kq.DiemChuyenCan = diemChuyenCan;
                kq.DiemCuoiKhoa = diemCuoiKhoa;
                kq.DiemTongKet = (diemChuyenCan * 0.3) + (diemCuoiKhoa * 0.7);
                kq.NhanXet = nhanXet;
                kq.KetQua = ketQua == "pass" ? "Hoàn thành" : "Không hoàn thành";
                kq.NgayCapNhat = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã lưu thành công điểm và cập nhật kết quả!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy kết quả học tập.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
