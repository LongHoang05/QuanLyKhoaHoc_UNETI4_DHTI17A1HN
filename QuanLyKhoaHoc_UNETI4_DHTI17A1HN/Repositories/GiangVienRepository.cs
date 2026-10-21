using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học & Giảng viên
    // Repository triển khai IGiangVienRepository
    // ========================================================
    public class GiangVienRepository : IGiangVienRepository
    {
        private readonly ApplicationDbContext _context;

        public GiangVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GiangVien>> GetAllAsync()
        {
            return await _context.GiangViens
                .AsNoTracking()
                .OrderBy(g => g.HoTen)
                .ToListAsync();
        }

        public async Task<GiangVien?> GetByIdAsync(int id)
        {
            return await _context.GiangViens.FindAsync(id);
        }

        public async Task AddAsync(GiangVien giangVien)
        {
            await _context.GiangViens.AddAsync(giangVien);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GiangVien giangVien)
        {
            _context.GiangViens.Update(giangVien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var gv = await _context.GiangViens.FindAsync(id);
            if (gv != null)
            {
                _context.GiangViens.Remove(gv);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.GiangViens.AnyAsync(g => g.MaGiangVien == id);
        }
    }
}
