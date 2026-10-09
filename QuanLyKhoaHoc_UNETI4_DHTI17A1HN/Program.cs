// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - cấu hình DbContext và Session dùng chung

using Microsoft.EntityFrameworkCore;                  // [THÊM]
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;           // [THÊM]

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // [THÊM] DbContext (SQL Server) dùng chung cho cả nhóm
            builder.Services.AddDbContext<KhoaHocDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // [THÊM] Session: lưu MaTaiKhoan, HoTen, VaiTro khi đăng nhập
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            // ===================== ĐĂNG KÝ DỊCH VỤ TỪNG MODULE =====================
            // Mỗi người CHỈ thêm dòng vào vùng của mình. Ví dụ: builder.Services.AddScoped<ILopHocService, LopHocService>();

            // ===== MODULE 1 (Vũ Đình Trường) =====

            // ===== MODULE 2 (Nguyễn Đình Kiên) =====

            // ===== MODULE 3 (Nguyễn Đức Linh) =====

            // ===== MODULE 4 (Nguyễn Hoàng Long) =====

            // ===== MODULE 5 (Nguyễn Tùng Dương) =====

            // ===================== HẾT VÙNG ĐĂNG KÝ DỊCH VỤ =====================

            var app = builder.Build();
            // Tự áp Migration và nạp dữ liệu mẫu khi khởi động
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<KhoaHocDbContext>();
                TrinhNapDuLieu.NapDuLieu(db);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseSession();   // [THÊM] phải đặt sau UseRouting và trước UseAuthorization

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}