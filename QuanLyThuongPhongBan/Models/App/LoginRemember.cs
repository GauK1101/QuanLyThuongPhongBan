// Models/LoginRememberModel.cs
namespace QuanLyThuongPhongBan.Models;

public class LoginRemember
{
    public string? Username { get; set; }
    public string? EncryptedPassword { get; set; }
    public bool RememberMe { get; set; }
}