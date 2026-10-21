# BÀI TẬP LỚN MÔN THỰC HÀNH LẬP TRÌNH .NET
## ĐỀ TÀI: QUẢN LÝ ĐĂNG KÝ KHÓA HỌC VÀ HỌC VIÊN TRUNG TÂM ĐÀO TẠO
- **Trường:** Đại học Kinh tế - Kỹ thuật Công nghiệp (UNETI)
- **Khoa:** Công nghệ Thông tin
- **Lớp:** DHTI17A1HN
- **Nhóm thực hiện:** Nhóm UNETI 4
- **Công nghệ nền tảng:** .NET 10 | ASP.NET Core 10 MVC | Entity Framework Core 10 | Microsoft SQL Server | Bootstrap 5
- **Mô hình kiến trúc:** **Repository Pattern & Service Layer** (Theo đúng giáo trình thực hành khoa CNTT UNETI)
- **Công cụ phát triển chuẩn:** **Visual Studio 2022 (Visual Studio Tím)**

---

## 🎯 1. TỔNG QUAN KHUNG DỰ ÁN (STARTER FRAMEWORK) ĐÃ CÓ SẴN
Khung dự án này là **nền tảng chung duy nhất** đã được cấu hình hoàn chỉnh để tất cả thành viên trong nhóm dùng chung mà không bị lỗi hay xung đột mã nguồn.

### Những thành phần ĐÃ ĐƯỢC LÀM SẴN trong khung:
1. **Toàn bộ Entity Models (`Models/*.cs`):**
   - Đầy đủ 7 model chuẩn theo đặc tả Đề tài BTL: `TaiKhoan`, `ChuongTrinhDaoTao`, `LopHoc`, `HocVien`, `DangKyHoc`, `BuoiHoc`, `KetQuaHocTap`.
   - Đã khai báo đầy đủ khóa chính, khóa ngoại, DataAnnotations (Validation, MaxLength, DisplayName).
2. **Cơ sở dữ liệu & Cấu hình quan hệ (`Data/ApplicationDbContext.cs`):**
   - Đã cấu hình quan hệ 1-N (ChuongTrinhDaoTao -> LopHoc -> DangKyHoc...), 1-1 (DangKyHoc -> KetQuaHocTap).
   - Đã cấu hình sẵn **Seed Data** (tạo sẵn dữ liệu mẫu cho tài khoản test, chương trình đào tạo, lớp học, học viên).
3. **Migration nền tảng gốc (`Migrations/*_InitialCreate.cs`):**
   - Đã tạo sẵn 1 migration duy nhất cho toàn bộ 7 bảng cốt lõi.
4. **Cấu hình ứng dụng (`Program.cs`, `appsettings.json`):**
   - Đã bật sẵn kết nối SQL Server, Session 60 phút, Routing MVC.
5. **Giao diện dùng chung (`Views/Shared/_Layout.cshtml`, `Views/Home/Index.cshtml`):**
   - Đã có thanh điều hướng chuẩn, thông báo TempData và trang chủ hiển thị phân công nhiệm vụ.

---

## ⚠️ 2. NGUYÊN TẮC VÀNG VỀ MIGRATION (CHỐNG XUNG ĐỘT TRÊN GITHUB)

> [!IMPORTANT]
> **TẤT CẢ 5 THÀNH VIÊN BẮT BUỘC PHẢI ĐỌC KỸ PHẦN NÀY:**
> 1. Trưởng nhóm đã tạo sẵn Migration nền tảng `InitialCreate` và push lên GitHub.
> 2. **TUYỆT ĐỐI KHÔNG BẠN NÀO ĐƯỢC CHẠY LỆNH `Add-Migration`**. Nếu mỗi người tự ý gõ `Add-Migration`, thư mục `Migrations/` sẽ bị xung đột (conflict) mã SHA khi merge Git và làm hỏng dự án.
> 3. Sau khi clone dự án về máy, mỗi thành viên **CHỈ CẦN CHẠY DUY NHẤT LỆNH UPDATE DATABASE**.

### Cách cập nhật Database trong Visual Studio Tím:
1. Mở file giải pháp `QuanLyKhoaHoc_UNETI4_DHTI17A1HN.slnx` bằng **Visual Studio Tím**.
2. Kiểm tra chuỗi kết nối trong `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=QuanLyKhoaHoc_UNETI4_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```
   *(Nếu máy bạn dùng SQL Server Express, đổi `Server=.` thành `Server=.\\SQLEXPRESS`)*.
3. Vào menu: **Tools** -> **NuGet Package Manager** -> **Package Manager Console (PMC)**.
4. Gõ lệnh sau và nhấn Enter:
   ```powershell
   Update-Database
   ```
   *(Hoặc nếu dùng Terminal / CMD trong thư mục project thì gõ: `dotnet ef database update`)*.

👉 Ngay lập tức SQL Server sẽ tạo Database `QuanLyKhoaHoc_UNETI4_DB` kèm đầy đủ tất cả các bảng và nạp sẵn dữ liệu mẫu. Cả nhóm dùng chung dữ liệu mà không ai bị đè file của ai!

---

## 🏗️ 3. HƯỚNG DẪN CHI TIẾT VỀ REPOSITORY PATTERN & SERVICE LAYER

> [!IMPORTANT]
> **TIÊU CHÍ CHẤM ĐIỂM BẮT BUỘC CỦA THẦY CÔ:**
> - **Controller KHÔNG ĐƯỢC truy vấn trực tiếp `DbContext`** (Ví dụ: cấm viết `_context.LopHocs.ToList()` trong Controller). Nếu viết trực tiếp sẽ bị trừ điểm kiến trúc.
> - **Repository Layer:** Chuyên trách lấy và lưu dữ liệu vào CSDL thông qua EF Core + LINQ.
> - **Service Layer:** Chuyên trách nghiệp vụ, validation, logic kiểm tra (trùng lịch, hết hạn, quá sĩ số) và gọi Repository.
> - **Controller Layer:** Chỉ tiếp nhận Request từ View, gọi Service lấy kết quả và trả về View.

### Mô hình luồng dữ liệu chuẩn:
```text
[ View (.cshtml) ] 
       ↕ (Gửi Form / Nhận ViewModel)
[ Controller ]
       ↕ (Gọi xử lý nghiệp vụ)
[ Service Layer ] (Kiểm tra hợp lệ, tính điểm, check trùng lịch, check sĩ số)
       ↕ (Gọi truy vấn dữ liệu)
[ Repository Layer ] (EF Core 10, LINQ, Include dữ liệu bảng liên kết)
       ↕
[ ApplicationDbContext ] 
       ↕
[ SQL Server Database ]
```

---

### 🚀 Quy trình 5 bước để tự tạo Repository & Service cho Module của mình:

#### Bước 1: Tạo Interface Repository trong thư mục `Repositories/`
Tạo file Interface định nghĩa các phương thức cần truy vấn CSDL.
*Ví dụ:* `Repositories/ILopHocRepository.cs`
```csharp
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface ILopHocRepository
    {
        Task<IEnumerable<LopHoc>> GetAllAsync();
        Task<LopHoc?> GetByIdAsync(int id);
        Task<LopHoc?> GetByIdWithDetailsAsync(int id); // Include GiangVien & KhoaHoc
        Task AddAsync(LopHoc lopHoc);
        Task UpdateAsync(LopHoc lopHoc);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Truy vấn LINQ nghiệp vụ: Tìm kiếm, lọc, sắp xếp, phân trang Skip & Take
        Task<(IEnumerable<LopHoc> Items, int TotalCount)> GetPagedAsync(
            string? keyword, string? trangThai, int? maGiangVien, string? sortBy, int pageIndex, int pageSize);
    }
}
```

#### Bước 2: Tạo Lớp Repository cài đặt Interface trong thư mục `Repositories/`
Nhận `ApplicationDbContext` qua Constructor để thực hiện truy vấn LINQ.
*Ví dụ:* `Repositories/LopHocRepository.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public class LopHocRepository : ILopHocRepository
    {
        private readonly ApplicationDbContext _context;

        public LopHocRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LopHoc>> GetAllAsync()
        {
            return await _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiangVien)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LopHoc?> GetByIdAsync(int id)
        {
            return await _context.LopHocs.FindAsync(id);
        }

        public async Task<LopHoc?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiangVien)
                .Include(l => l.DangKyHocs)
                .FirstOrDefaultAsync(l => l.MaLopHoc == id);
        }

        public async Task AddAsync(LopHoc lopHoc)
        {
            await _context.LopHocs.AddAsync(lopHoc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LopHoc lopHoc)
        {
            _context.LopHocs.Update(lopHoc);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var lop = await _context.LopHocs.FindAsync(id);
            if (lop != null)
            {
                _context.LopHocs.Remove(lop);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.LopHocs.AnyAsync(l => l.MaLopHoc == id);
        }

        public async Task<(IEnumerable<LopHoc> Items, int TotalCount)> GetPagedAsync(
            string? keyword, string? trangThai, int? maGiangVien, string? sortBy, int pageIndex, int pageSize)
        {
            var query = _context.LopHocs
                .Include(l => l.KhoaHoc)
                .Include(l => l.GiangVien)
                .AsQueryable();

            // 1. Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(l => l.TenLop.Contains(keyword) || l.MaLop.Contains(keyword) || l.KhoaHoc.TenKhoaHoc.Contains(keyword));
            }

            // 2. Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(l => l.TrangThai == trangThai);
            }

            // 3. Lọc theo giảng viên
            if (maGiangVien.HasValue)
            {
                query = query.Where(l => l.MaGiangVien == maGiangVien.Value);
            }

            // 4. Sắp xếp LINQ
            query = sortBy switch
            {
                "ten_desc" => query.OrderByDescending(l => l.TenLop),
                "ngay_asc" => query.OrderBy(l => l.NgayKhaiGiang),
                "ngay_desc" => query.OrderByDescending(l => l.NgayKhaiGiang),
                "hocphi_asc" => query.OrderBy(l => l.HocPhi),
                "hocphi_desc" => query.OrderByDescending(l => l.HocPhi),
                _ => query.OrderBy(l => l.TenLop)
            };

            int totalCount = await query.CountAsync();

            // 5. Phân trang trên Server bằng Skip & Take
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
```

#### Bước 3: Tạo Service Layer trong thư mục `Services/`
Service chứa các kiểm tra nghiệp vụ và gọi Repository.
*Ví dụ:* `Services/ILopHocService.cs` & `Services/LopHocService.cs`
```csharp
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    public interface ILopHocService
    {
        Task<(IEnumerable<LopHoc> Items, int TotalPages, int CurrentPage)> GetDanhSachLopAsync(
            string? keyword, string? trangThai, int? maGiangVien, string? sortBy, int pageIndex);
        Task<LopHoc?> GetChiTietLopAsync(int id);
        Task<(bool Success, string? ErrorMessage)> TaoLopHocAsync(LopHoc lopHoc);
        Task<(bool Success, string? ErrorMessage)> CapNhatLopHocAsync(LopHoc lopHoc);
        Task<bool> XoaLopHocAsync(int id);
    }

    public class LopHocService : ILopHocService
    {
        private readonly ILopHocRepository _lopHocRepo;
        private const int PageSize = 10;

        public LopHocService(ILopHocRepository lopHocRepo)
        {
            _lopHocRepo = lopHocRepo;
        }

        public async Task<(IEnumerable<LopHoc> Items, int TotalPages, int CurrentPage)> GetDanhSachLopAsync(
            string? keyword, string? trangThai, int? maGiangVien, string? sortBy, int pageIndex)
        {
            if (pageIndex < 1) pageIndex = 1;
            var (items, totalCount) = await _lopHocRepo.GetPagedAsync(keyword, trangThai, maGiangVien, sortBy, pageIndex, PageSize);
            int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            return (items, totalPages, pageIndex);
        }

        public async Task<LopHoc?> GetChiTietLopAsync(int id)
        {
            return await _lopHocRepo.GetByIdWithDetailsAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage)> TaoLopHocAsync(LopHoc lopHoc)
        {
            // Nghiệp vụ Validation logic
            if (lopHoc.HanDangKy < lopHoc.NgayMoDangKy)
                return (false, "Hạn đăng ký phải sau hoặc bằng ngày mở đăng ký.");

            if (lopHoc.NgayKhaiGiang < lopHoc.HanDangKy)
                return (false, "Ngày khai giảng phải sau ngày hết hạn đăng ký.");

            if (lopHoc.SiSoToiDa <= 0)
                return (false, "Sĩ số tối đa phải lớn hơn 0.");

            await _lopHocRepo.AddAsync(lopHoc);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatLopHocAsync(LopHoc lopHoc)
        {
            if (!await _lopHocRepo.ExistsAsync(lopHoc.MaLopHoc))
                return (false, "Lớp học không tồn tại.");

            await _lopHocRepo.UpdateAsync(lopHoc);
            return (true, null);
        }

        public async Task<bool> XoaLopHocAsync(int id)
        {
            if (!await _lopHocRepo.ExistsAsync(id)) return false;
            await _lopHocRepo.DeleteAsync(id);
            return true;
        }
    }
}
```

#### Bước 4: Đăng ký Dependency Injection trong `Program.cs`
Mở `Program.cs`, tìm khu vực đăng ký dịch vụ và thêm dòng `AddScoped`:
```csharp
// Đăng ký Repository & Service của Module 2 (Nguyễn Đình Kiên)
builder.Services.AddScoped<ILopHocRepository, LopHocRepository>();
builder.Services.AddScoped<ILopHocService, LopHocService>();
```

#### Bước 5: Tiêm (Inject) Service vào Controller qua Constructor
*Ví dụ:* `Controllers/LopHocController.cs`
```csharp
using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    public class LopHocController : Controller
    {
        private readonly ILopHocService _lopHocService;

        // Tiêm Service vào Constructor (Tuyệt đối không tiêm ApplicationDbContext vào đây)
        public LopHocController(ILopHocService lopHocService)
        {
            _lopHocService = lopHocService;
        }

        public async Task<IActionResult> Index(string? keyword, string? trangThai, int? maGiangVien, string? sortBy, int page = 1)
        {
            var (items, totalPages, currentPage) = await _lopHocService.GetDanhSachLopAsync(keyword, trangThai, maGiangVien, sortBy, page);
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.Keyword = keyword;
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LopHoc lopHoc)
        {
            if (!ModelState.IsValid) return View(lopHoc);

            var (success, error) = await _lopHocService.TaoLopHocAsync(lopHoc);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(lopHoc);
            }

            TempData["Success"] = "Tạo lớp học thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
```

---

## 📋 4. BẢNG PHÂN CHIA REPOSITORY & SERVICE CỤ THỂ CHO 5 THÀNH VIÊN

Mỗi bạn căn cứ vào bảng này để tạo đúng file và viết các hàm nghiệp vụ tương ứng:

| Thành viên | Module | File Repository cần tạo | File Service cần tạo | Các phương thức nghiệp vụ trọng tâm |
| :--- | :---: | :--- | :--- | :--- |
| **Vũ Đình Trường**<br>`23103100026` | **Module 1** | - `ITaiKhoanRepository.cs`<br>- `TaiKhoanRepository.cs`<br>- `IChuongTrinhDaoTaoRepository.cs`<br>- `ChuongTrinhDaoTaoRepository.cs` | - `ITaiKhoanService.cs`<br>- `TaiKhoanService.cs`<br>- `IChuongTrinhDaoTaoService.cs`<br>- `ChuongTrinhDaoTaoService.cs` | - `KiemTraDangNhap(tenDangNhap, matKhau)`<br>- `TaoTaiKhoanHocVien(taiKhoan)`<br>- `LuuSession(MaTaiKhoan, HoTen, VaiTro)`<br>- Phân quyền (Admin, GiaoVu, HocVien)<br>- CRUD Chương trình đào tạo |
| **Nguyễn Đình Kiên**<br>`23103100020` | **Module 2** | - `ILopHocRepository.cs`<br>- `LopHocRepository.cs` | - `ILopHocService.cs`<br>- `LopHocService.cs` | - `GetPagedAsync(keyword, trangThai, sapXep, page, size)`<br>- **LINQ Skip() & Take() phân trang server**<br>- Validation ngày mở, hạn nộp, ngày khai giảng<br>- Quản lý lớp học (CRUD) |
| **Nguyễn Đức Linh**<br>`23103100042` | **Module 3** | - `IHocVienRepository.cs`<br>- `HocVienRepository.cs`<br>- `IDangKyHocRepository.cs`<br>- `DangKyHocRepository.cs` | - `IHocVienService.cs`<br>- `HocVienService.cs` | - `LayHoSoTheoTaiKhoan(maTaiKhoan)`<br>- `CapNhatHoSo(hocVien)`<br>- `LayDanhSachLopDangMo()`<br>- `DangKyLop(maHocVien, maLop)`: Kiểm tra bằng LINQ: lớp còn mở, còn chỗ, không trùng đơn |
| **Nguyễn Hoàng Long**<br>`23103100046` | **Module 4** | - `IQuanLyDangKyRepository.cs`<br>- `QuanLyDangKyRepository.cs` | - `IQuanLyDangKyService.cs`<br>- `QuanLyDangKyService.cs` | - `LayDanhSachDonDangKy(maLop, trangThai)`<br>- **LINQ đếm sĩ số lớp đã duyệt < SiSoToiDa**<br>- `DuyetDon(maDangKy, ghiChu)`<br>- `TuChoiDon(maDangKy, lyDo)`<br>- `XacNhanDongHocPhi(maDangKy, soTien, ngayDong)` |
| **Nguyễn Tùng Dương**<br>`23103100003` | **Module 5** | - `IBuoiHocRepository.cs`<br>- `BuoiHocRepository.cs`<br>- `IKetQuaHocTapRepository.cs`<br>- `KetQuaHocTapRepository.cs` | - `ILichHocService.cs`<br>- `LichHocService.cs`<br>- `IThongKeService.cs`<br>- `ThongKeService.cs` | - **Thuật toán LINQ kiểm tra trùng lịch:** Không trùng phòng, không trùng GV, không trùng lớp trong cùng khung giờ<br>- Quản lý điểm & xếp loại học tập<br>- **Dashboard thống kê LINQ**: Doanh thu, Tỷ lệ đăng ký, Sĩ số các lớp |

---

## 💻 5. HƯỚNG DẪN THỰC THI TRÊN VISUAL STUDIO 2022 (TÍM)

### Bước 1: Mở Solution
1. Mở **Visual Studio 2022**.
2. Chọn **Open a project or solution**.
3. Trỏ tới file: `QuanLyKhoaHoc_UNETI4_DHTI17A1HN.slnx` (hoặc `.sln`).

### Bước 2: Tạo nhánh cá nhân trong Visual Studio Tím
1. Nhìn xuống góc dưới cùng bên phải cửa sổ Visual Studio, bấm vào tên branch hiện tại (thường là `main`).
2. Chọn **New Branch...**
3. Đặt tên nhánh theo đúng quy định:
   - Trường: `feature/module-1-truong`
   - Kiên: `feature/module-2-kien`
   - Linh: `feature/module-3-linh`
   - Long: `feature/module-4-long`
   - Dương: `feature/module-5-duong`
4. Bấm **Create**.

### Bước 3: Cập nhật Database
1. Bấm tổ hợp phím hoặc vào: `Tools` -> `NuGet Package Manager` -> `Package Manager Console`.
2. Gõ:
   ```powershell
   Update-Database
   ```
3. Đợi hiển thị `Done.` là database đã sẵn sàng chạy.

### Bước 4: Viết code và Thêm file mới
- Tạo file Interface / Class trong `Repositories/` và `Services/`.
- Mở `Program.cs` đăng ký `builder.Services.AddScoped<..., ...>();`.
- Tạo Controller trong thư mục `Controllers/` (Inject Service vào Constructor).
- Tạo View trong thư mục `Views/[TênController]/`.

### Bước 5: Commit và Push code lên GitHub trong Visual Studio Tím
1. Mở cửa sổ **Git Changes** (nhấn `Ctrl + 0, Ctrl + G` hoặc vào menu `View` -> `Git Changes`).
2. Nhập nội dung commit theo đúng cú pháp bắt buộc của môn học:
   ```text
   [Mã SV] [Module] Tóm tắt công việc đã làm
   ```
   **Ví dụ chuẩn:**
   - `[23103100026] [Module 1] Hoan thanh dang nhap va phan quyen Session`
   - `[23103100020] [Module 2] Viet repository va service phan trang LINQ Skip Take`
   - `[23103100042] [Module 3] Viet service kiem tra dieu kien dang ky lop`
   - `[23103100046] [Module 4] Viet logic kiem soat si so va thu hoc phi`
   - `[23103100003] [Module 5] Viet thuat toan LINQ check trung lich va thong ke`
3. Bấm **Commit All** (hoặc `Commit All and Push`).
4. Lên GitHub tạo **Pull Request (PR)** để Trưởng nhóm kiểm tra và gộp vào nhánh chính `main`.

---

## 🔑 6. THÔNG TIN TÀI KHOẢN MẪU (SEED DATA CÓ SẴN TRONG CSDL)

Dữ liệu đã được nạp sẵn khi chạy `Update-Database`:

| Tài khoản (Username) | Mật khẩu | Vai trò (Role) | Chức năng kiểm thử |
| :--- | :--- | :--- | :--- |
| `admin` | `123456` | **Admin** | Quản trị toàn bộ hệ thống, tài khoản |
| `giaovu` | `123456` | **GiaoVu** | Mở lớp, duyệt đơn đăng ký, thu học phí, xếp lịch |
| `hocvien` | `123456` | **HocVien** | Cập nhật hồ sơ, đăng ký lớp học, xem lịch sử |

---

## 🎓 7. BỘ CÂU HỎI VẤN ĐÁP THẦY CÔ THƯỜNG HỎI KHI BẢO VỆ BTL

- **Sinh viên 1 (Vũ Đình Trường - M1):**
  1. *Cơ chế lưu thông tin đăng nhập trong Session hoạt động ra sao? Lưu những gì?*
  2. *Làm thế nào để chặn sinh viên/người dùng chưa đăng nhập không gõ URL vào trang Admin được?*
  3. *Tại sao phải tách `TaiKhoanRepository` và `TaiKhoanService` thay vì code trực tiếp trong Controller?*
- **Sinh viên 2 (Nguyễn Đình Kiên - M2):**
  1. *Chỉ ra vị trí code LINQ kết hợp Search + Filter + Sort + Phân trang trong `LopHocRepository`?*
  2. *Tại sao phải dùng `Skip()` và `Take()` trên `IQueryable` trước khi gọi `.ToListAsync()` thay vì gọi `.ToList()` rồi mới cắt trang?*
  3. *Giữ lại các tham số lọc và từ khóa tìm kiếm khi bấm qua trang 2, 3 bằng cách nào trên Razor View?*
- **Sinh viên 3 (Nguyễn Đức Linh - M3):**
  1. *Khi học viên bấm đăng ký một lớp, trong `HocVienService` kiểm tra những điều kiện nghiệp vụ gì?*
  2. *Viết câu lệnh LINQ kiểm tra học viên không thể gửi 2 đơn đăng ký cho cùng 1 lớp?*
- **Sinh viên 4 (Nguyễn Hoàng Long - M4):**
  1. *Viết câu lệnh LINQ đếm số lượng học viên đã duyệt của lớp để so sánh với `SiSoToiDa` trước khi cho phép duyệt đơn?*
  2. *Luồng xử lý từ lúc đăng ký đến lúc đóng tiền học phí và vào lớp diễn ra như thế nào qua Service và Repository?*
- **Sinh viên 5 (Nguyễn Tùng Dương - M5):**
  1. *Giải thích điều kiện logic kiểm tra 2 khoảng thời gian học bị trùng nhau (Overlap check)?*
  2. *Trình bày công thức tính điểm tổng kết và cách dùng LINQ (`GroupBy`, `Count`, `Sum`, `Average`) trong Service để tạo số liệu cho Dashboard?*
