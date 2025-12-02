using QuanLyThuongPhongBan.Models.App;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface ICurrentUserService
    {
        AppUser? CurrentUser { get; }
        bool IsInRole(string role);
        void SetCurrentUser(AppUser? user);
        void Logout();
    }
}
