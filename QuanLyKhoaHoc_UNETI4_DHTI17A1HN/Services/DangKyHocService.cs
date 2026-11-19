// ========================================================
// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Phụ trách: Module 4 - Đăng ký khóa học, Quản lý hồ sơ & Thu học phí
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    public class DangKyHocService : IDangKyHocService
    {
        private readonly IDangKyHocRepository _dangKyHocRepo;
        private readonly IBuoiHocRepository _buoiHocRepo;

        public DangKyHocService(
            IDangKyHocRepository dangKyHocRepo,
            IBuoiHocRepository buoiHocRepo)
        {
            _dangKyHocRepo = dangKyHocRepo;
            _buoiHocRepo = buoiHocRepo;
        }

        public Task<(IEnumerable<DangKyHoc> Items, int TotalCount)> GetPagedAsync(
            string? search,
            int? maChuongTrinh,
            int? maLop,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sortBy,
            int page,
            int pageSize)
        {
            return _dangKyHocRepo.GetPagedAsync(
                search, maChuongTrinh, maLop, trangThai, tuNgay, denNgay, sortBy, page, pageSize);
        }

        public Task<(int ChoDuyetCount, int DaDuyetCount, double TongDoanhThu)> GetThongKeNhanhAsync()
        {
            return _dangKyHocRepo.GetThongKeNhanhAsync();
        }

        public Task<DangKyHoc?> GetByIdWithDetailsAsync(int id)
        {
            return _dangKyHocRepo.GetByIdWithDetailsAsync(id);
        }

        public async Task<(bool Success, string Message)> DuyetDangKyAsync(int id, string? nhanXet, string nguoiDuyet)
        {
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);
            if (dk == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký.");
            }

            // Kiểm tra sĩ số tối đa của lớp học trước khi duyệt
            if (dk.LopHoc != null)
            {
                int currentEnrolled = await _dangKyHocRepo.CountEnrolledInLopAsync(dk.MaLopHoc);
                if (currentEnrolled >= dk.LopHoc.SiSoToiDa)
                {
                    return (false, $"Không thể duyệt hồ sơ #{id}! Lớp '{dk.LopHoc.TenLop}' đã đủ sĩ số tối đa ({dk.LopHoc.SiSoToiDa} học viên).");
                }
            }

            // Kiểm tra trùng thời khóa biểu của học viên giữa các lớp
            var (hasScheduleConflict, conflictMsg) = await _dangKyHocRepo.KiemTraTrungLichHocVienAsync(dk.MaHocVien, dk.MaLopHoc);
            if (hasScheduleConflict)
            {
                return (false, $"Không thể duyệt hồ sơ #{id}! {conflictMsg}");
            }

            dk.TrangThai = "Đã duyệt";
            dk.NgayXuLy = DateTime.Now;
            dk.NhanXetGiaoVu = string.IsNullOrWhiteSpace(nhanXet) ? $"Đã duyệt bởi {nguoiDuyet}" : nhanXet.Trim();

            await _dangKyHocRepo.UpdateAsync(dk);
            return (true, $"Đã duyệt hồ sơ đăng ký #{id:D3} của học viên {dk.HocVien?.HoTen} thành công!");
        }

        public async Task<(bool Success, string Message)> XacNhanHocPhiAsync(int id, string? phuongThuc, string? ghiChu, string staffName)
        {
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);
            if (dk == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký.");
            }

            decimal hocPhiLop = dk.LopHoc?.HocPhi ?? 0m;
            if (dk.TrangThai == "Đã đóng học phí" || (hocPhiLop > 0 && dk.SoTienDaDong >= hocPhiLop))
            {
                return (false, "Học viên này đã đóng đủ 100% học phí của lớp.");
            }

            // Kiểm tra sĩ số nếu hồ sơ trước đó chưa nằm trong nhóm đã duyệt
            if (dk.TrangThai == "Chờ duyệt" && dk.LopHoc != null)
            {
                int enrolledCount = await _dangKyHocRepo.CountEnrolledInLopAsync(dk.MaLopHoc);
                if (enrolledCount >= dk.LopHoc.SiSoToiDa)
                {
                    return (false, $"Không thể thu tiền! Lớp '{dk.LopHoc.TenLop}' đã đạt sĩ số tối đa ({dk.LopHoc.SiSoToiDa} học viên).");
                }
            }

            // Thu đúng 1 lần 100% học phí của lớp
            dk.SoTienDaDong = hocPhiLop;
            dk.NgayDongHocPhi = DateTime.Now;
            dk.TrangThai = "Đã đóng học phí";

            string method = !string.IsNullOrWhiteSpace(phuongThuc) ? $"[{phuongThuc.Trim()}] " : "";
            dk.GhiChu = string.IsNullOrWhiteSpace(ghiChu)
                ? $"{method}Đã thu đủ 100% học phí ({hocPhiLop:N0} đ) bởi {staffName}"
                : $"{method}{ghiChu.Trim()} (Thu bởi: {staffName})";

            if (dk.NgayXuLy == null)
            {
                dk.NgayXuLy = DateTime.Now;
                dk.NhanXetGiaoVu = staffName;
            }

            await _dangKyHocRepo.UpdateAsync(dk);
            return (true, $"Xác nhận thu đủ 100% học phí ({hocPhiLop:N0} đ) cho học viên {dk.HocVien?.HoTen} thành công!");
        }

        public async Task<(bool Success, string Message)> HoanTacChuaNhanHocPhiAsync(int id, string? ghiChu, string staffName)
        {
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);
            if (dk == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký.");
            }

            if (dk.TrangThai != "Chờ xác nhận học phí")
            {
                return (false, $"Chỉ có thể hoàn tác khi hồ sơ đang ở trạng thái 'Chờ xác nhận học phí' (hiện tại: '{dk.TrangThai}').");
            }

            dk.TrangThai = "Đã duyệt";
            dk.SoTienDaDong = 0m;
            dk.NgayDongHocPhi = null;

            string lyDo = string.IsNullOrWhiteSpace(ghiChu)
                ? "Giáo vụ đối soát: Chưa nhận được tiền học phí, vui lòng kiểm tra và thực hiện nộp lại"
                : ghiChu.Trim();
            dk.GhiChu = $"[Chưa nhận học phí] {lyDo} ({staffName} - {DateTime.Now:dd/MM/yyyy HH:mm})";

            await _dangKyHocRepo.UpdateAsync(dk);
            return (true, $"Đã trả hồ sơ #{id:D3} của học viên {dk.HocVien?.HoTen} về trạng thái 'Đã duyệt' (Chưa nộp học phí).");
        }

        public async Task<(bool Success, string Message)> BatDauHocAsync(int id)
        {
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);
            if (dk == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký.");
            }

            bool isPaidOrFreeApproved = dk.TrangThai == "Đã đóng học phí" ||
                (dk.LopHoc != null && dk.LopHoc.HocPhi <= 0 && dk.TrangThai == "Đã duyệt");
            if (!isPaidOrFreeApproved)
            {
                return (false, $"Không thể kích hoạt vào học! Hồ sơ đang ở trạng thái '{dk.TrangThai}', học viên cần hoàn tất nộp học phí trước.");
            }

            dk.TrangThai = "Đang học";
            await _dangKyHocRepo.UpdateAsync(dk);
            return (true, $"Đã chuyển học viên {dk.HocVien?.HoTen} sang trạng thái Đang học!");
        }

        public async Task<(bool Success, string Message)> TuChoiDangKyAsync(int id, string lyDo, string staffName)
        {
            var dk = await _dangKyHocRepo.GetByIdAsync(id);
            if (dk == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký.");
            }

            if (string.IsNullOrWhiteSpace(lyDo))
            {
                return (false, "Vui lòng nhập lý do từ chối hồ sơ.");
            }

            dk.TrangThai = "Từ chối";
            dk.NgayXuLy = DateTime.Now;
            dk.NhanXetGiaoVu = lyDo.Trim();
            dk.GhiChu = $"[Từ chối bởi {staffName} vào {DateTime.Now:dd/MM/yyyy HH:mm}]: {lyDo.Trim()}";

            await _dangKyHocRepo.UpdateAsync(dk);
            return (true, $"Đã từ chối hồ sơ #{id:D3} thành công!");
        }

        public async Task<(bool Success, string Message)> HuyDangKyAsync(int id, string? lyDo, string? vaiTro, string staffName)
        {
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);
            if (dk == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký.");
            }

            // Ràng buộc nghiệp vụ: Không được tự ý hủy khi lớp đã có lịch học/đang diễn ra nếu không có sự đồng ý của Admin
            bool coLichHoc = await _buoiHocRepo.HasLichHocLopChuaHuyAsync(dk.MaLopHoc);
            if (coLichHoc && dk.TrangThai == "Đang học" && vaiTro != "Admin")
            {
                return (false, "Lớp học đã lên lịch và đang học. Chỉ Quản trị viên (Admin) mới có quyền hủy hồ sơ trong giai đoạn này.");
            }

            dk.TrangThai = "Đã hủy";
            dk.GhiChu = $"[Hủy bởi {staffName} lúc {DateTime.Now:dd/MM/yyyy HH:mm}]: {(string.IsNullOrWhiteSpace(lyDo) ? "Người dùng yêu cầu" : lyDo.Trim())}";

            await _dangKyHocRepo.UpdateAsync(dk);
            return (true, $"Đã hủy hồ sơ đăng ký #{id} thành công!");
        }

        public async Task<(bool Success, string Message)> XoaDangKyAsync(int id)
        {
            var dk = await _dangKyHocRepo.GetByIdAsync(id);
            if (dk == null)
            {
                return (false, "Không tìm thấy hồ sơ cần xóa.");
            }

            if (!IsDeletableStatus(dk.TrangThai))
            {
                return (false, $"Không thể xóa hồ sơ #{id:D3} vì đang ở trạng thái '{dk.TrangThai}'. Vui lòng chuyển trạng thái sang 'Đã hủy' trước.");
            }

            await _dangKyHocRepo.DeleteAsync(id);
            return (true, $"Đã xóa hồ sơ đăng ký #{id:D3} khỏi hệ thống.");
        }

        public async Task<(bool Success, string Message, int DeletedCount, int SkippedCount)> XoaNhieuDangKyAsync(IEnumerable<int>? ids)
        {
            var idList = ids?.ToList();
            if (idList == null || idList.Count == 0)
            {
                return (false, "Vui lòng chọn ít nhất 1 hồ sơ đăng ký để xóa.", 0, 0);
            }

            int deleted = 0;
            int skipped = 0;

            foreach (var id in idList)
            {
                var dk = await _dangKyHocRepo.GetByIdAsync(id);
                if (dk == null) continue;

                if (!IsDeletableStatus(dk.TrangThai))
                {
                    skipped++;
                    continue;
                }

                await _dangKyHocRepo.DeleteAsync(id);
                deleted++;
            }

            if (deleted == 0 && skipped > 0)
            {
                return (false, $"Không thể xóa {skipped} hồ sơ đã chọn do đang ở trạng thái đã duyệt/đang học. Vui lòng chuyển sang 'Đã hủy' trước.", 0, skipped);
            }

            string msg = $"Đã xóa thành công {deleted} hồ sơ đăng ký học.";
            if (skipped > 0)
            {
                msg += $" Bỏ qua {skipped} hồ sơ đang ở trạng thái Đã duyệt/Đang học.";
            }

            return (true, msg, deleted, skipped);
        }

        private static bool IsDeletableStatus(string? trangThai)
        {
            return trangThai == "Chờ duyệt" || trangThai == "Từ chối" || trangThai == "Đã hủy";
        }
    }
}
