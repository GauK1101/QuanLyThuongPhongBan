using QuanLyThuongPhongBan.ViewModels;

namespace QuanLyThuongPhongBan.Models;

public partial class TbPhongBan : BaseViewModel
{
    public int Id { get => Get<int>(); set => Set(value); }

    public string TenPhongBan { get => Get<string>() ?? string.Empty; set => Set(value); }

    public string MoTa { get => Get<string>() ?? string.Empty; set => Set(value); }
}
