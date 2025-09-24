using QuanLyThuongPhongBan.ViewModels;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongSmb : BaseViewModel
{
    public int Id
    {
        get => Get<int>();
        set => Set(value);
    }

    public decimal TongGiaTriSmb
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal TiLeQuyetToan
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal TongTiLeSmb
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal TongGiaTriSmbDieuChinh
    {
        get => Get<decimal>();
        set => Set(value);
    }

}
