// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// Áp dụng: Session & Cookie Authentication theo Slide 14-32 Bài giảng Bảo mật UNETI
// ========================================================

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters
{
    /// <summary>
    /// Attribute phân quyền kết hợp Session & Cookie Authentication (ClaimsPrincipal).
    /// Kiểm tra quyền trực tiếp tại Controller theo đúng yêu cầu mục 5.4 của đề tài:
    /// "Các chức năng quản trị phải kiểm tra quyền tại Controller trước khi xử lý; không được chỉ ẩn menu hoặc nút trên View."
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public SessionAuthorizeAttribute(params string[] roles)
        {
            _allowedRoles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 1. Cho phép bỏ qua kiểm tra nếu Action được gán attribute [AllowAnonymous]
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
            {
                base.OnActionExecuting(context);
                return;
            }

            var session = context.HttpContext.Session;
            var maTaiKhoan = session.GetInt32("MaTaiKhoan");
            var vaiTro = session.GetString("VaiTro");

            // 2. Cơ chế đồng bộ từ Cookie Authentication Claims sang Session (Slide 14-32 Bài giảng Bảo mật)
            if (maTaiKhoan == null && context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var idClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                var nameClaim = context.HttpContext.User.Identity?.Name;
                var hoTenClaim = context.HttpContext.User.FindFirst("HoTen")?.Value;
                var emailClaim = context.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                if (int.TryParse(idClaim, out int uid))
                {
                    maTaiKhoan = uid;
                    vaiTro = roleClaim ?? "";
                    session.SetInt32("MaTaiKhoan", uid);
                    session.SetString("VaiTro", vaiTro);
                    if (!string.IsNullOrEmpty(nameClaim)) session.SetString("TenDangNhap", nameClaim);
                    if (!string.IsNullOrEmpty(hoTenClaim)) session.SetString("HoTen", hoTenClaim);
                    if (!string.IsNullOrEmpty(emailClaim)) session.SetString("Email", emailClaim);
                }
            }

            // 3. Chưa đăng nhập
            if (maTaiKhoan == null || string.IsNullOrEmpty(vaiTro))
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { returnUrl });
                return;
            }

            // 4. Kiểm tra vai trò nếu có chỉ định danh sách vai trò cho phép
            if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(vaiTro, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
