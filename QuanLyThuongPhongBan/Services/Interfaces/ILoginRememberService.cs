using QuanLyThuongPhongBan.Models;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface ILoginRememberService
    {
        LoginRemember LoadRememberedLogin();
        void SaveRememberedLogin(LoginRemember model);
        void ClearRememberedLogin();
    }
}
