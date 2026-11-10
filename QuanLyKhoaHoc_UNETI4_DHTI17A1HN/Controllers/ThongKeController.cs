// ========================================================
// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Phụ trách: Module 5 - Buổi học, Điểm danh, Kết quả học tập & Thống kê báo cáo
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    [SessionAuthorize("Admin", "GiaoVu")]
    public class ThongKeController : Controller
    {
        private readonly IThongKeRepository _thongKeRepo;
        private readonly IDangKyHocRepository _dangKyHocRepo;
        private readonly IBuoiHocRepository _buoiHocRepo;
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IChuongTrinhDaoTaoRepository _ctdtRepo;
        private readonly IHocVienRepository _hocVienRepo;
        private readonly IKetQuaHocTapRepository _ketQuaRepo;

        public ThongKeController(
            IThongKeRepository thongKeRepo,
            IDangKyHocRepository dangKyHocRepo,
            IBuoiHocRepository buoiHocRepo,
            ILopHocRepository lopHocRepo,
            IChuongTrinhDaoTaoRepository ctdtRepo,
            IHocVienRepository hocVienRepo,
            IKetQuaHocTapRepository ketQuaRepo)
        {
            _thongKeRepo = thongKeRepo;
            _dangKyHocRepo = dangKyHocRepo;
            _buoiHocRepo = buoiHocRepo;
            _lopHocRepo = lopHocRepo;
            _ctdtRepo = ctdtRepo;
            _hocVienRepo = hocVienRepo;
            _ketQuaRepo = ketQuaRepo;
        }

        public async Task<IActionResult> Dashboard(int page = 1, string? status = "all", string? search = "")
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (string.Equals(vaiTro, "GiangVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "GiangVien");
            }
            if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("TienTrinh", "CongHocVien");
            }

            int pageSize = 5;
            if (page < 1) page = 1;
            status = string.IsNullOrWhiteSpace(status) ? "all" : status.Trim();
            search = string.IsNullOrWhiteSpace(search) ? "" : search.Trim();

            string? filterTrangThai = null;
            if (status == "Chờ duyệt") filterTrangThai = "Chờ duyệt";
            else if (status == "Đã duyệt") filterTrangThai = "Đã duyệt";

            var (recentActivities, totalActivities) = await _dangKyHocRepo.GetPagedAsync(
                search,
                null,
                null,
                filterTrangThai,
                null,
                null,
                "ngay_desc",
                page,
                pageSize);

            int totalPages = (int)Math.Ceiling(totalActivities / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages)
            {
                page = totalPages;
                (recentActivities, totalActivities) = await _dangKyHocRepo.GetPagedAsync(
                    search, null, null, filterTrangThai, null, null, "ngay_desc", page, pageSize);
            }

            var allBuoiHocs = (await _buoiHocRepo.GetAllAsync()).ToList();
            var danhSachBuoiHoc = allBuoiHocs
                .Where(b => b.TrangThai != "Đã hủy" && b.ThoiGianBatDau >= DateTime.Today)
                .OrderBy(b => b.ThoiGianBatDau)
                .Take(5)
                .ToList();
            if (!danhSachBuoiHoc.Any())
            {
                danhSachBuoiHoc = allBuoiHocs
                    .Where(b => b.TrangThai != "Đã hủy")
                    .OrderByDescending(b => b.ThoiGianBatDau)
                    .Take(5)
                    .ToList();
            }

            var (tongSoDiem, soDat, _, _) = await _ketQuaRepo.GetThongKePhoiDiemAsync();
            double tyLeHT = tongSoDiem > 0 ? Math.Round((double)soDat / tongSoDiem * 100, 1) : 0;

            var kpis = await _thongKeRepo.GetTongHopKpiAsync();

            var vm = new DashboardVM
            {
                TongSoCTDT = (int)kpis["TongSoCTDT"],
                TongSoLop = (int)kpis["TongSoLop"],
                LopDangMo = (int)kpis["LopDangMo"],
                TongSoHocVien = (int)kpis["TongSoHocVien"],
                TongSoDangKy = (int)kpis["TongSoDangKy"],
                DangKyChoDuyet = (int)kpis["DangKyChoDuyet"],
                DangKyDaDuyet = (int)kpis["DangKyDaDuyet"],
                BuoiHocSapToi = (int)kpis["BuoiHocSapToi"],
                HocVienHoanThanh = (int)kpis["HocVienHoanThanh"],
                TyLeHoanThanh = tyLeHT,
                DanhSachBuoiHoc = danhSachBuoiHoc,
                HoatDongGanDay = recentActivities.ToList(),
                CurrentPage = page,
                TotalPages = totalPages,
                TotalActivities = totalActivities,
                PageSize = pageSize,
                SelectedStatus = status,
                Search = search
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DuyetNhanh(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var dangKy = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);
            if (dangKy != null)
            {
                if (dangKy.TrangThai != "Chờ duyệt")
                {
                    string statusMsg = $"Hồ sơ này đang ở trạng thái '{dangKy.TrangThai}', không thể duyệt nhanh lại.";
                    if (isAjax) return Json(new { success = false, message = statusMsg });
                    TempData["Error"] = statusMsg;
                    return RedirectToAction(nameof(Dashboard));
                }

                if (dangKy.LopHoc != null)
                {
                    int currentEnrolled = await _dangKyHocRepo.CountEnrolledInLopAsync(dangKy.MaLopHoc);
                    if (currentEnrolled >= dangKy.LopHoc.SiSoToiDa)
                    {
                        string msg = $"Không thể duyệt! Lớp '{dangKy.LopHoc.TenLop}' đã đủ sĩ số tối đa ({dangKy.LopHoc.SiSoToiDa} học viên).";
                        if (isAjax) return Json(new { success = false, message = msg });
                        TempData["Error"] = msg;
                        return RedirectToAction(nameof(Dashboard));
                    }
                }

                dangKy.TrangThai = "Đã duyệt";
                dangKy.NgayXuLy = DateTime.Now;
                dangKy.NhanXetGiaoVu = HttpContext.Session.GetString("HoTen") ?? "Ban Quản trị";
                await _dangKyHocRepo.UpdateAsync(dangKy);
                if (isAjax)
                {
                    return Json(new { success = true, message = "Đã phê duyệt đăng ký thành công!" });
                }
            }
            else
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
                }
            }
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (string.Equals(vaiTro, "GiangVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "GiangVien");
            }
            if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("TienTrinh", "CongHocVien");
            }

            // 1. Số lớp học theo chương trình đào tạo (Bài tập 12 - GroupBy & Count)
            ViewBag.LopTheoCTDT = await _thongKeRepo.GetLopTheoCTDTAsync();

            // 2. Số đăng ký theo từng lớp (Bài tập 12 - GroupBy & Count)
            ViewBag.DKTheoLop = await _thongKeRepo.GetDKTheoLopAsync();

            // 3. Số đăng ký theo trạng thái
            ViewBag.DKTheoTrangThai = await _thongKeRepo.GetDKTheoTrangThaiAsync();

            // 4. Lớp có nhiều đăng ký nhất (Bài tập 14 - Min/Max)
            ViewBag.LopNhieuDKNhat = await _thongKeRepo.GetLopNhieuDKNhatAsync();

            // 5. Doanh thu theo tháng (Bài tập 13 - Sum & GroupBy)
            ViewBag.DoanhThuTheoThang = await _thongKeRepo.GetDoanhThuTheoThangAsync();

            // 6. Doanh thu theo chương trình đào tạo
            ViewBag.DoanhThuTheoCTDT = await _thongKeRepo.GetDoanhThuTheoCTDTAsync();

            // 7. Số đăng ký theo tháng
            ViewBag.DKTheoThang = await _thongKeRepo.GetDKTheoThangAsync();

            // 8. Số buổi học theo tháng
            ViewBag.BuoiHocTheoThang = await _thongKeRepo.GetBuoiHocTheoThangAsync();

            // 9. Điểm tổng kết trung bình theo lớp
            ViewBag.DiemTBTheoLop = await _thongKeRepo.GetDiemTBTheoLopAsync();

            // 10. Tỷ lệ lấp đầy sĩ số
            var tyLe = await _thongKeRepo.GetTyLeLapDayAsync();
            ViewBag.TyLeLapDay = tyLe;

            // 11. Tỷ lệ Hoàn thành
            ViewBag.TyLeHoanThanh = await _thongKeRepo.GetTyLeHoanThanhKhoaHocAsync();

            // 12. Thống kê nâng cao phục vụ Dashboard & KPI (Bài tập 15)
            var kpis = await _thongKeRepo.GetTongHopKpiAsync();
            ViewBag.TongDoanhThu = (double)kpis["TongDoanhThu"];
            ViewBag.TongBuoiHoc = (int)kpis["TongBuoiHoc"];
            ViewBag.DangKyChoDuyet = (int)kpis["DangKyChoDuyet"];
            ViewBag.AvgLapDay = tyLe.Any() ? Math.Round(tyLe.Average(t => t.TyLe), 1) : 0;
            ViewBag.TongChoHoc = (int)kpis["TongChoHoc"];
            ViewBag.ChoDaDangKy = (int)kpis["ChoDaDangKy"];

            // 13. Bảng xếp hạng giảng viên phụ trách
            ViewBag.TopGiangVien = await _thongKeRepo.GetTopGiangVienAsync();

            return View();
        }
    }
}
