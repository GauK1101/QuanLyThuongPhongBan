using QuanLyThuongPhongBan.ViewModels;

namespace QuanLyThuongPhongBan.Models;

public partial class TbTaiKhoan : BaseViewModel
{
    public int Id
    {
        get => Get<int>(); 
        set => Set(value);
    }
    public int IdPhongBan
    {
        get => Get<int>();
        set => Set(value);
    }
    public string? TenDangNhap
    {
        get => Get<string?>();
        set => Set(value);
    }

    public string? MatKhau
    {
        get => Get<string?>();
        set => Set(value);
    }

    public string? HoTen
    {
        get => Get<string?>();
        set => Set(value);
    }
}
