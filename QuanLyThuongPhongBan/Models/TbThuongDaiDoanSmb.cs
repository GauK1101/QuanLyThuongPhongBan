using QuanLyThuongPhongBan.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongDaiDoanSmb : BaseViewModel
{
    public int Id
    {
        get => Get<int>();
        set => Set(value);
    }

    [ForeignKey("IdPhongBanNavigation")]
    public int IdPhongBan
    {
        get => Get<int>();
        set => Set(value);
    }

    public int IdThuongSmb
    {
        get => Get<int>();
        set => Set(value);
    }

    public decimal? TiLeTongSmb
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? DoanhThuXuatHoaDon
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? DoanhThuHopDong
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? GiaTriTongSmb
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? TiLeDot1
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? GiaTriDot1
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? TiLeThuHoiCongNo
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? ThuHoiCongNo
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? NghiemThu
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    [ForeignKey(nameof(IdThuongSmb))]
    public TbThuongSmb? TbThuongSmb { get; set; }

    public virtual TbPhongBan? IdPhongBanNavigation { get; set; }
}
