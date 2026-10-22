// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using System.Security.Cryptography;
using System.Text;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Helpers
{
    /// <summary>
    /// Lớp hỗ trợ mã hóa và xác thực mật khẩu
    /// Hỗ trợ cả mật khẩu băm SHA-256 lẫn mật khẩu văn bản gốc (tương thích seed data)
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Băm mật khẩu bằng thuật toán SHA-256
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Xác thực mật khẩu: kiểm tra trùng khớp với mật khẩu băm hoặc mật khẩu gốc
        /// </summary>
        public static bool VerifyPassword(string inputPassword, string storedPassword)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedPassword))
                return false;

            // Kiểm tra khớp trực tiếp (hỗ trợ seed data gốc như admin123, 123456)
            if (inputPassword == storedPassword)
                return true;

            // Kiểm tra khớp với chuỗi băm SHA-256
            var hashedInput = HashPassword(inputPassword);
            return hashedInput == storedPassword;
        }
    }
}
