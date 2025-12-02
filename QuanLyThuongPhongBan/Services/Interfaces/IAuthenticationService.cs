using QuanLyThuongPhongBan.Models.App;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AppUser?> LoginAsync(string username, string password);
        Task<bool> ChangePasswordAsync(string username, string oldPass, string newPass);
    }
}
