using QuanLyThuongPhongBan.Models.App;
using QuanLyThuongPhongBan.Services.Interfaces;

namespace QuanLyThuongPhongBan.Services.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private AppUser? _currentUser;

        public AppUser? CurrentUser => _currentUser;

        public bool IsInRole(string role)
            => _currentUser != null &&
               _currentUser.Role.Equals(role, StringComparison.OrdinalIgnoreCase);

        public void SetCurrentUser(AppUser? user)
        {
            _currentUser = user;
        }

        public void Logout() => SetCurrentUser(null);
    }
}
