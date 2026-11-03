using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================================
// 1. Cấu hình DbContext kết nối SQL Server
// ========================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=QuanLyKhoaHoc_UNETI4_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ========================================================
// 2. Cấu hình Session & Cookie Authentication cho đăng nhập / phân quyền (Theo bài giảng Bảo mật UNETI)
// ========================================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình Cookie Authentication với Claims (Slide 14-32 Bài giảng Bảo mật)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/TaiKhoan/DangNhap";
        options.LogoutPath = "/TaiKhoan/DangXuat";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.Name = "UNETI_Auth_Cookie";
    });

builder.Services.AddHttpContextAccessor();

// ========================================================
// 3. Đăng ký Dependency Injection (Repository & Service cho cả 5 Thành viên Nhóm 4)
// ========================================================
// Module 1 - Vũ Đình Trường (23103100026): Tài khoản, Đăng nhập, Phân quyền & Chương trình đào tạo
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<IChuongTrinhDaoTaoRepository, ChuongTrinhDaoTaoRepository>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddScoped<IChuongTrinhDaoTaoService, ChuongTrinhDaoTaoService>();

// Module 2 - Nguyễn Đình Kiên (23103100020): Quản lý lớp học, Tìm kiếm, Lọc, Sắp xếp và Phân trang
builder.Services.AddScoped<ILopHocRepository, LopHocRepository>();
builder.Services.AddScoped<ILopHocService, LopHocService>();

// Module 3 - Nguyễn Đức Linh (23103100042): Quản lý học viên, Hồ sơ cá nhân và Cổng học viên
builder.Services.AddScoped<IHocVienRepository, HocVienRepository>();
builder.Services.AddScoped<IHocVienService, HocVienService>();

// Module 4 - Nguyễn Hoàng Long (23103100046): Đăng ký khóa học, Quản lý hồ sơ & Thu học phí
builder.Services.AddScoped<IDangKyHocRepository, DangKyHocRepository>();

// Module 5 - Nguyễn Tùng Dương (23103100003): Lịch học, Điểm danh, Kết quả học tập & Dashboard Thống kê
builder.Services.AddScoped<IBuoiHocRepository, BuoiHocRepository>();
builder.Services.AddScoped<IKetQuaHocTapRepository, KetQuaHocTapRepository>();
builder.Services.AddScoped<IThongKeRepository, ThongKeRepository>();

// Module Giảng viên (Portal & Quản lý) - Bổ sung theo yêu cầu giảng viên hướng dẫn
builder.Services.AddScoped<IGiangVienRepository, GiangVienRepository>();
builder.Services.AddScoped<IGiangVienService, GiangVienService>();

// Đăng ký dịch vụ Controllers with Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Tự động kiểm tra / khởi tạo database và nạp dữ liệu mẫu phong phú (Seed Mock Data)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        // Tự động sửa lỗi schema: Bổ sung bảng GiangVien và cột MaGiangVien vào LopHoc, BuoiHoc nếu Database chưa có
        context.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GiangVien')
            BEGIN
                CREATE TABLE [dbo].[GiangVien] (
                    [MaGiangVien] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [MaTaiKhoan] INT NULL,
                    [HoTen] NVARCHAR(200) NOT NULL,
                    [Email] NVARCHAR(200) NOT NULL,
                    [SoDienThoai] NVARCHAR(20) NULL,
                    [HocVi] NVARCHAR(100) NULL,
                    [ChuyenNganh] NVARCHAR(200) NULL,
                    [GioiThieu] NVARCHAR(500) NULL,
                    [TrangThai] BIT NOT NULL DEFAULT 1,
                    [NgayTao] DATETIME2 NOT NULL DEFAULT GETDATE(),
                    CONSTRAINT [FK_GiangVien_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [dbo].[TaiKhoan] ([MaTaiKhoan]) ON DELETE SET NULL
                );
            END;

            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LopHoc') AND name = 'MaGiangVien')
            BEGIN
                ALTER TABLE [dbo].[LopHoc] ADD [MaGiangVien] INT NULL;
            END;

            IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LopHoc_GiangVien_MaGiangVien')
            BEGIN
                ALTER TABLE [dbo].[LopHoc] ADD CONSTRAINT [FK_LopHoc_GiangVien_MaGiangVien] FOREIGN KEY ([MaGiangVien]) REFERENCES [dbo].[GiangVien] ([MaGiangVien]) ON DELETE SET NULL;
            END;

            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('BuoiHoc') AND name = 'MaGiangVien')
            BEGIN
                ALTER TABLE [dbo].[BuoiHoc] ADD [MaGiangVien] INT NULL;
            END;

            IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_BuoiHoc_GiangVien_MaGiangVien')
            BEGIN
                ALTER TABLE [dbo].[BuoiHoc] ADD CONSTRAINT [FK_BuoiHoc_GiangVien_MaGiangVien] FOREIGN KEY ([MaGiangVien]) REFERENCES [dbo].[GiangVien] ([MaGiangVien]) ON DELETE SET NULL;
            END;
        ");

        DbSeeder.SeedMockData(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lưu ý khi kết nối Database: Đảm bảo SQL Server đang chạy và chuỗi kết nối DefaultConnection chính xác.");
    }
}

// ========================================================
// 4. Cấu hình Middleware HTTP pipeline
// ========================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Bật Cookie Authentication & Session theo chuẩn bài giảng ASP.NET Core & Bảo mật UNETI
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

// Route thân thiện cho Khám phá khóa học
app.MapControllerRoute(
    name: "khampha",
    pattern: "KhamPha",
    defaults: new { controller = "CongHocVien", action = "DangKy" });

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
