using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học & Giảng viên
    // Repository triển khai IGiangVienRepository (In-Memory)
    // Lưu ý: GiangVien không có bảng DB riêng trong phiên bản này.
    // Dữ liệu giảng viên được lưu in-memory và TenGiangVien được
    // đồng bộ vào cột LopHoc.TenGiangVien khi tạo/cập nhật lớp học.
    // ========================================================
    public class GiangVienRepository : IGiangVienRepository
    {
        // Danh sách giảng viên cố định dùng cho Module 2
        // TenGiangVien sẽ được lưu vào bảng LopHoc khi chọn
        private static readonly List<GiangVien> _danhSachGiangVien = new()
        {
            new GiangVien
            {
                MaGiangVien = 1,
                HoTen = "ThS. Trần Văn Hùng",
                ChuyenMon = "Lập trình .NET & Web",
                Email = "hung.tran@uneti.edu.vn",
                SoDienThoai = "0912345678"
            },
            new GiangVien
            {
                MaGiangVien = 2,
                HoTen = "TS. Nguyễn Thị Lan",
                ChuyenMon = "Khoa học Dữ liệu & Trí tuệ Nhân tạo",
                Email = "lan.nguyen@uneti.edu.vn",
                SoDienThoai = "0923456789"
            },
            new GiangVien
            {
                MaGiangVien = 3,
                HoTen = "ThS. Lê Minh Đức",
                ChuyenMon = "An toàn Thông tin & Bảo mật",
                Email = "duc.le@uneti.edu.vn",
                SoDienThoai = "0934567890"
            },
            new GiangVien
            {
                MaGiangVien = 4,
                HoTen = "PGS.TS. Phạm Quốc Hùng",
                ChuyenMon = "Mạng máy tính & Hệ thống phân tán",
                Email = "hung.pham@uneti.edu.vn",
                SoDienThoai = "0945678901"
            },
            new GiangVien
            {
                MaGiangVien = 5,
                HoTen = "ThS. Bùi Thị Thảo",
                ChuyenMon = "Phân tích dữ liệu & Business Intelligence",
                Email = "thao.bui@uneti.edu.vn",
                SoDienThoai = "0956789012"
            },
        };

        public Task<IEnumerable<GiangVien>> GetAllAsync()
        {
            IEnumerable<GiangVien> result = _danhSachGiangVien
                .OrderBy(g => g.HoTen)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<GiangVien?> GetByIdAsync(int id)
        {
            GiangVien? result = _danhSachGiangVien
                .FirstOrDefault(g => g.MaGiangVien == id);
            return Task.FromResult(result);
        }

        public Task AddAsync(GiangVien giangVien)
        {
            // In-memory: thêm vào danh sách tạm thời
            _danhSachGiangVien.Add(giangVien);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(GiangVien giangVien)
        {
            // In-memory: cập nhật trong danh sách tạm thời
            var existing = _danhSachGiangVien
                .FirstOrDefault(g => g.MaGiangVien == giangVien.MaGiangVien);
            if (existing != null)
            {
                existing.HoTen = giangVien.HoTen;
                existing.ChuyenMon = giangVien.ChuyenMon;
                existing.Email = giangVien.Email;
                existing.SoDienThoai = giangVien.SoDienThoai;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            // In-memory: xóa khỏi danh sách tạm thời
            var gv = _danhSachGiangVien.FirstOrDefault(g => g.MaGiangVien == id);
            if (gv != null) _danhSachGiangVien.Remove(gv);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(int id)
        {
            bool result = _danhSachGiangVien.Any(g => g.MaGiangVien == id);
            return Task.FromResult(result);
        }
    }
}
