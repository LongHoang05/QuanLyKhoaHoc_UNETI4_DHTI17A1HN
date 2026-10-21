// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters
{
    /// <summary>
    /// Attribute phân quyền dựa trên Session.
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
            var session = context.HttpContext.Session;
            var maTaiKhoan = session.GetInt32("MaTaiKhoan");
            var vaiTro = session.GetString("VaiTro");

            // 1. Chưa đăng nhập
            if (maTaiKhoan == null || string.IsNullOrEmpty(vaiTro))
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { returnUrl });
                return;
            }

            // 2. Kiểm tra vai trò nếu có chỉ định danh sách vai trò cho phép
            if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(vaiTro, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
