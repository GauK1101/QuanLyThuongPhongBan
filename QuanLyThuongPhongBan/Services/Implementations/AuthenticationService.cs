using QuanLyThuongPhongBan.Models.App;
using QuanLyThuongPhongBan.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyThuongPhongBan.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        // Giả lập database bằng List (sau này thay bằng DbContext.InvoiceUsers.ToList())
        private readonly List<AppUser> _fakeDb = new()
        {
            new AppUser { Id = 1, Username = "admin",     PasswordHash = HashPassword("admin123"), DisplayName = "Quản trị viên",      Role = "Admin" },
            new AppUser { Id = 2, Username = "hosoduan", PasswordHash = HashPassword("hsda123456"),  DisplayName = "Hò sơ dự án",     Role = "hsda" },
            new AppUser { Id = 3, Username = "ketoan",  PasswordHash = HashPassword("kt123456"),   DisplayName = "kế toán",       Role = "kt" },
        };

        public async Task<AppUser?> LoginAsync(string username, string password)
        {
            var hash = HashPassword(password);
            var user = _fakeDb.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == hash &&
                u.IsActive);

            return user;
        }

        public async Task<bool> ChangePasswordAsync(string username, string oldPass, string newPass)
        {
            await Task.Delay(300);
            var user = _fakeDb.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null || user.PasswordHash != HashPassword(oldPass)) return false;

            user.PasswordHash = HashPassword(newPass);
            return true;
        }

        // SHA256 hash – an toàn, không lưu mật khẩu gốc
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
