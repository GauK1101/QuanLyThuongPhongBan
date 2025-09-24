using QuanLyThuongPhongBan.ViewModels;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongDaiDoanSmb : BaseViewModel
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

    public int IdTongThuongSmb
    {
        get => Get<int>();
        set => Set(value);
    }

    public decimal TiLeSmb
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal TiLeGiaTri
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal TiLeDot1
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal GiaTriDot1
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal ThuHoiCongNo
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal NghiemThu
    {
        get => Get<decimal>();
        set => Set(value);
    }
}
