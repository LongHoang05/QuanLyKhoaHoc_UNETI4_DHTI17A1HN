using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================================
// 1. Cấu hình DbContext kết nối SQL Server
// ========================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=.\\SQLEXPRESS;Database=QuanLyKhoaHoc_UNETI4_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ========================================================
// 2. Cấu hình Session & Cookie cho đăng nhập / phân quyền
// ========================================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// ========================================================
// 3. Đăng ký Dependency Injection (DI) - Module 1 (Vũ Đình Trường - 23103100026)
// ========================================================
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<IChuongTrinhDaoTaoRepository, ChuongTrinhDaoTaoRepository>();
builder.Services.AddScoped<IGiangVienRepository, GiangVienRepository>();

builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddScoped<IChuongTrinhDaoTaoService, ChuongTrinhDaoTaoService>();
builder.Services.AddScoped<IGiangVienService, GiangVienService>();

// Đăng ký dịch vụ Controllers with Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

// Bật Session (bắt buộc đặt trước UseAuthorization)
app.UseSession();

app.UseAuthorization();

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
