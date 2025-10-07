using QuanLyThuongPhongBan.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models;

public partial class TbNhatKy : BaseViewModel
{
    public int Id
    {
        get => Get<int>();
        set => Set(value);
    }

    public string? IdTaiKhoan
    {
        get => Get<string?>();
        set => Set(value);
    }

    public string? HanhDong
    {
        get => Get<string?>();
        set => Set(value);
    }

    public DateTime? ThoiGian
    {
        get => Get<DateTime?>();
        set => Set(value);
    }

    public string? MoTa
    {
        get => Get<string?>();
        set => Set(value);
    }

    //public virtual TbTaiKhoan? IdTaiKhoanNavigation { get; set; }
}
