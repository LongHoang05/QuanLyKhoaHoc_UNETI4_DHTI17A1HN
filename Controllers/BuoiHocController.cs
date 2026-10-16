// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Xử lý nghiệp vụ xếp lịch học, thuật toán chống trùng lịch (Module 5)

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class BuoiHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BuoiHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var buoiHocs = await _context.BuoiHocs
                .Include(b => b.LopHoc)
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();

            // Kiểm tra xung đột trùng lịch bằng LINQ
            int soLoiTrung = 0;
            for (int i = 0; i < buoiHocs.Count; i++)
            {
                for (int j = i + 1; j < buoiHocs.Count; j++)
                {
                    var b1 = buoiHocs[i];
                    var b2 = buoiHocs[j];
                    if (b1.TrangThai != "Đã hủy" && b2.TrangThai != "Đã hủy")
                    {
                        if ((b1.PhongHoc == b2.PhongHoc || b1.GiangVien == b2.GiangVien || b1.MaLop == b2.MaLop) &&
                            (b1.ThoiGianBatDau < b2.ThoiGianKetThuc && b1.ThoiGianKetThuc > b2.ThoiGianBatDau))
                        {
                            soLoiTrung++;
                        }
                    }
                }
            }

            ViewBag.SoLoiTrung = soLoiTrung;
            ViewBag.TotalRecords = buoiHocs.Count;
            ViewBag.SoThi = buoiHocs.Count(b => b.NoiDung != null && (b.NoiDung.ToLower().Contains("thi") || b.NoiDung.ToLower().Contains("kiểm tra")));
            ViewBag.SoChinhKhoa = buoiHocs.Count(b => b.TrangThai != "Đã hủy");
            ViewBag.LopHocList = await _context.LopHocs.ToListAsync();

            // Danh sách phòng học và giảng viên để hiển thị bộ lọc
            ViewBag.DanhSachPhong = buoiHocs.Select(b => b.PhongHoc).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            ViewBag.DanhSachGiangVien = buoiHocs.Select(b => b.GiangVien).Where(g => !string.IsNullOrEmpty(g)).Distinct().ToList();

            return View(buoiHocs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuoiHoc buoiHoc)
        {
            // 1. Kiểm tra lớp có ít nhất 1 đăng ký "Đã đóng học phí" không?
            bool coHocVienDongHocPhi = _context.DangKyHocs
                .Any(dk => dk.MaLop == buoiHoc.MaLop && dk.TrangThai == "Đã đóng học phí");
            
            if (!coHocVienDongHocPhi)
            {
                ModelState.AddModelError("", "Lớp này chưa có học viên nào đóng học phí, không được xếp lịch.");
            }

            // 2. Validate thời gian hợp lệ
            if (buoiHoc.ThoiGianKetThuc <= buoiHoc.ThoiGianBatDau)
            {
                ModelState.AddModelError("ThoiGianKetThuc", "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }
            if (buoiHoc.ThoiGianBatDau < DateTime.Now)
            {
                ModelState.AddModelError("ThoiGianBatDau", "Thời gian học không được nằm trong quá khứ.");
            }

            // 3. Kiểm tra số buổi học vượt quy định
            var lopHoc = await _context.LopHocs.FindAsync(buoiHoc.MaLop);
            if (lopHoc != null)
            {
                int soBuoiChuaHuy = _context.BuoiHocs
                    .Count(b => b.MaLop == buoiHoc.MaLop && b.TrangThai != "Đã hủy");
                
                if (soBuoiChuaHuy >= lopHoc.SoBuoiHoc)
                {
                    ModelState.AddModelError("", $"Lớp này đã xếp đủ {lopHoc.SoBuoiHoc} buổi học theo quy định.");
                }
            }

            // 4. KIỂM TRA TRÙNG LỊCH BẰNG LINQ (Lớp, Phòng, Giảng viên)
            bool isTrungLich = _context.BuoiHocs.Any(b =>
                b.TrangThai != "Đã hủy" &&
                (b.MaLop == buoiHoc.MaLop || b.PhongHoc == buoiHoc.PhongHoc || b.GiangVien == buoiHoc.GiangVien) &&
                (buoiHoc.ThoiGianBatDau < b.ThoiGianKetThuc && buoiHoc.ThoiGianKetThuc > b.ThoiGianBatDau)
            );

            if (isTrungLich)
            {
                ModelState.AddModelError("", "Lịch bị trùng! Kiểm tra lại Giảng viên, Phòng học hoặc Lớp đã có lịch trong khung giờ này.");
            }

            if (ModelState.IsValid)
            {
                buoiHoc.TrangThai = "Đã lên lịch";
                _context.Add(buoiHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xếp lịch học thành công!";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["Error"] = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index));
        }

        // --- YÊU CẦU 9.3: CẬP NHẬT SAU BUỔI HỌC ---
        [HttpPost]
        public async Task<IActionResult> HoanThanh(int id)
        {
            var buoiHoc = await _context.BuoiHocs.FindAsync(id);
            if (buoiHoc == null) return NotFound();

            if (buoiHoc.TrangThai == "Đã hủy")
            {
                TempData["Error"] = "Không thể hoàn thành buổi học đã hủy.";
                return RedirectToAction(nameof(Index));
            }
            
            if (buoiHoc.ThoiGianKetThuc > DateTime.Now)
            {
                TempData["Error"] = "Buổi học chưa kết thúc (hoặc chưa diễn ra), không thể đánh dấu hoàn thành.";
                return RedirectToAction(nameof(Index));
            }

            buoiHoc.TrangThai = "Đã hoàn thành";
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Đã cập nhật trạng thái hoàn thành thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Huy(int id)
        {
            var buoiHoc = await _context.BuoiHocs.FindAsync(id);
            if (buoiHoc == null) return NotFound();

            buoiHoc.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Đã hủy ca học thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
