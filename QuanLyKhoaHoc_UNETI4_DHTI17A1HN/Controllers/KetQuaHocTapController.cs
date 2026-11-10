// ========================================================
// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Phụ trách: Module 5 - Buổi học, Điểm danh, Kết quả học tập & Thống kê báo cáo
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    [SessionAuthorize("Admin", "GiaoVu")]
    public class KetQuaHocTapController : Controller
    {
        private readonly IKetQuaHocTapRepository _ketQuaRepo;
        private readonly IDangKyHocRepository _dangKyHocRepo;
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IBuoiHocRepository _buoiHocRepo;

        public KetQuaHocTapController(
            IKetQuaHocTapRepository ketQuaRepo,
            IDangKyHocRepository dangKyHocRepo,
            ILopHocRepository lopHocRepo,
            IBuoiHocRepository buoiHocRepo)
        {
            _ketQuaRepo = ketQuaRepo;
            _dangKyHocRepo = dangKyHocRepo;
            _lopHocRepo = lopHocRepo;
            _buoiHocRepo = buoiHocRepo;
        }

        public async Task<IActionResult> Index(int? maLop, string? ketQua, string? search, string? sortBy, int page = 1)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("TienTrinh", "CongHocVien", new { tab = "ketqua" });
            }

            int pageSize = 8;
            if (page < 1) page = 1;

            // 1. Phân trang & Lọc LINQ qua Repository
            var (ketQuas, totalRecords) = await _ketQuaRepo.GetPagedAsync(
                search,
                maLop,
                null,
                ketQua,
                page,
                pageSize,
                sortBy);

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages)
            {
                page = totalPages;
                (ketQuas, totalRecords) = await _ketQuaRepo.GetPagedAsync(
                    search, maLop, null, ketQua, page, pageSize, sortBy);
            }

            // 2. Thống kê KPI & phổ điểm qua Repository
            var allScores = (await _ketQuaRepo.GetAllAsync()).ToList();
            int totalGraded = allScores.Count;
            int passCount = allScores.Count(k => k.KetQua == "Hoàn thành");
            int failCount = allScores.Count(k => k.KetQua == "Không hoàn thành");
            double avgScore = allScores.Any() ? Math.Round(allScores.Average(k => k.DiemTongKet), 2) : 0;
            double passRate = totalGraded > 0 ? Math.Round((double)passCount / totalGraded * 100, 1) : 0;

            ViewBag.TotalGraded = totalGraded;
            ViewBag.PassCount = passCount;
            ViewBag.FailCount = failCount;
            ViewBag.AverageScore = avgScore;
            ViewBag.PassRate = passRate;

            // Phổ điểm
            ViewBag.CountA = allScores.Count(d => d.DiemTongKet >= 8.5);
            ViewBag.CountB = allScores.Count(d => d.DiemTongKet >= 7.0 && d.DiemTongKet < 8.5);
            ViewBag.CountC = allScores.Count(d => d.DiemTongKet >= 5.5 && d.DiemTongKet < 7.0);
            ViewBag.CountD = allScores.Count(d => d.DiemTongKet >= 4.0 && d.DiemTongKet < 5.5);
            ViewBag.CountF = allScores.Count(d => d.DiemTongKet < 4.0);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedKetQua = ketQua;
            ViewBag.Search = search;
            ViewBag.SortBy = sortBy;

            // 3. Dropdowns (Chỉ liệt kê hồ sơ đủ điều kiện chấm điểm và chưa có kết quả)
            ViewBag.LopHocList = await _lopHocRepo.GetAllAsync();
            ViewBag.EligibleDangKys = (await _ketQuaRepo.GetEligibleDangKysForGradingAsync()).ToList();

            return View(ketQuas);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.EligibleDangKys = (await _ketQuaRepo.GetEligibleDangKysForGradingAsync()).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KetQuaHocTap kq)
        {
            ModelState.Remove("DangKyHoc");
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var dangKy = await _dangKyHocRepo.GetByIdWithDetailsAsync(kq.MaDangKy);

            if (dangKy == null)
            {
                ModelState.AddModelError("", "Không tìm thấy hồ sơ đăng ký hợp lệ.");
            }
            else
            {
                // Kiểm tra trạng thái hợp lệ để chấm điểm (Đã duyệt / Đã đóng học phí / Đang học)
                if (dangKy.TrangThai != "Đang học" && dangKy.TrangThai != "Đã đóng học phí" && dangKy.TrangThai != "Đã duyệt")
                {
                    ModelState.AddModelError("", $"Hồ sơ đăng ký này không ở trạng thái đủ điều kiện chấm điểm (Trạng thái hiện tại: {dangKy.TrangThai}).");
                }

                // Kiểm tra xem đã có kết quả trước đó chưa
                bool daCoDiem = await _ketQuaRepo.ExistsByDangKyIdAsync(kq.MaDangKy);
                if (daCoDiem)
                {
                    ModelState.AddModelError("", "Học viên này đã được ghi nhận kết quả học tập.");
                }

                // Kiểm tra lớp học đã hoàn thành toàn bộ buổi học chưa (Mục 9.4)
                var classSessions = (await _buoiHocRepo.GetByLopHocIdAsync(dangKy.MaLopHoc))
                    .Where(b => b.TrangThai != "Đã hủy")
                    .ToList();

                if (!classSessions.Any())
                {
                    ModelState.AddModelError("", "Lớp học này chưa có buổi học nào được lên lịch, không thể chấm điểm.");
                }
                else if (classSessions.Any(b => b.TrangThai != "Đã hoàn thành"))
                {
                    int chuaHoanThanh = classSessions.Count(b => b.TrangThai != "Đã hoàn thành");
                    ModelState.AddModelError("", $"Lớp học vẫn còn {chuaHoanThanh} buổi học chưa hoàn thành. Chỉ khi toàn bộ buổi học đã hoàn thành mới được ghi nhận điểm cuối khóa.");
                }
            }

            if (kq.DiemChuyenCan < 0 || kq.DiemChuyenCan > 10 || kq.DiemCuoiKhoa < 0 || kq.DiemCuoiKhoa > 10)
            {
                ModelState.AddModelError("", "Điểm chuyên cần và cuối khóa phải nằm trong khoảng từ 0 đến 10.");
            }

            if (ModelState.IsValid)
            {
                // Tự động tính điểm tổng kết: 40% Điểm quá trình + 60% Điểm thi cuối kỳ (Chuẩn quy chế BTL)
                kq.DiemTongKet = Math.Round((kq.DiemChuyenCan * 0.4) + (kq.DiemCuoiKhoa * 0.6), 2);
                if (string.IsNullOrEmpty(kq.KetQua) || kq.KetQua == "Chưa xét")
                {
                    kq.KetQua = kq.DiemTongKet >= 5.0 ? "Hoàn thành" : "Không hoàn thành";
                }
                kq.NgayCapNhat = DateTime.Now;

                await _ketQuaRepo.AddAsync(kq);

                // Cập nhật trạng thái đăng ký thành Hoàn thành hoặc Không hoàn thành
                if (dangKy != null)
                {
                    dangKy.TrangThai = kq.KetQua;
                    await _dangKyHocRepo.UpdateAsync(dangKy);
                }

                if (isAjax)
                {
                    return Json(new { success = true, message = "Ghi nhận kết quả học tập thành công!" });
                }
                TempData["Success"] = "Ghi nhận kết quả học tập thành công!";
                return RedirectToAction(nameof(Index));
            }

            string errorMsg = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            if (isAjax)
            {
                return Json(new { success = false, message = errorMsg });
            }
            TempData["Error"] = errorMsg;
            return RedirectToAction(nameof(Index));
        }

        // --- CẬP NHẬT ĐIỂM SỐ TỪ RIGHT SLIDE-OVER DRAWER ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int maKetQua, double diemChuyenCan, double diemCuoiKhoa, string? nhanXet, string? ketQua)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var kq = await _ketQuaRepo.GetByIdWithDetailsAsync(maKetQua);

            if (kq == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy bản ghi kết quả học tập." });
                TempData["Error"] = "Không tìm thấy bản ghi kết quả học tập.";
                return RedirectToAction(nameof(Index));
            }

            if (diemChuyenCan < 0 || diemChuyenCan > 10 || diemCuoiKhoa < 0 || diemCuoiKhoa > 10)
            {
                if (isAjax) return Json(new { success = false, message = "Điểm phải nằm trong khoảng từ 0 đến 10." });
                TempData["Error"] = "Điểm phải nằm trong khoảng từ 0 đến 10.";
                return RedirectToAction(nameof(Index));
            }

            kq.DiemChuyenCan = diemChuyenCan;
            kq.DiemCuoiKhoa = diemCuoiKhoa;
            kq.DiemTongKet = Math.Round((diemChuyenCan * 0.4) + (diemCuoiKhoa * 0.6), 2);
            kq.NhanXet = nhanXet ?? "";
            
            if (!string.IsNullOrEmpty(ketQua))
            {
                kq.KetQua = ketQua == "pass" || ketQua == "Hoàn thành" ? "Hoàn thành" : "Không hoàn thành";
            }
            else
            {
                kq.KetQua = kq.DiemTongKet >= 5.0 ? "Hoàn thành" : "Không hoàn thành";
            }
            kq.NgayCapNhat = DateTime.Now;

            if (kq.DangKyHoc != null)
            {
                kq.DangKyHoc.TrangThai = kq.KetQua;
            }

            await _ketQuaRepo.UpdateAsync(kq);

            if (isAjax) return Json(new { success = true, message = "Đã cập nhật thành công kết quả học tập!" });
            TempData["Success"] = "Đã cập nhật thành công kết quả học tập!";
            return RedirectToAction(nameof(Index));
        }

        // --- XÓA KẾT QUẢ TỪ DELETE MODAL ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var kq = await _ketQuaRepo.GetByIdWithDetailsAsync(id);
            if (kq == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy kết quả học tập để xóa." });
                TempData["Error"] = "Không tìm thấy kết quả học tập để xóa.";
                return RedirectToAction(nameof(Index));
            }

            if (kq.DangKyHoc != null)
            {
                kq.DangKyHoc.TrangThai = "Đang học";
                await _dangKyHocRepo.UpdateAsync(kq.DangKyHoc);
            }

            await _ketQuaRepo.DeleteAsync(id);

            if (isAjax) return Json(new { success = true, message = "Đã xóa bản ghi kết quả học tập và hoàn lại trạng thái Đang học cho học viên!" });
            TempData["Success"] = "Đã xóa bản ghi kết quả học tập và hoàn lại trạng thái Đang học cho học viên!";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // ACTION DELETE MULTIPLE: XÓA NHIỀU KẾT QUẢ HỌC TẬP
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int>? ids)
        {
            if (ids == null || !ids.Any())
                return Json(new { success = false, message = "Vui lòng chọn ít nhất 1 kết quả học tập để xóa." });

            int deleted = 0;
            foreach (var id in ids)
            {
                var kq = await _ketQuaRepo.GetByIdWithDetailsAsync(id);
                if (kq != null)
                {
                    if (kq.DangKyHoc != null)
                    {
                        kq.DangKyHoc.TrangThai = "Đang học";
                        await _dangKyHocRepo.UpdateAsync(kq.DangKyHoc);
                    }
                    await _ketQuaRepo.DeleteAsync(id);
                    deleted++;
                }
            }

            return Json(new { success = true, message = $"Đã xóa thành công {deleted} kết quả học tập và hoàn lại trạng thái cho học viên.", deletedCount = deleted });
        }
    }
}
