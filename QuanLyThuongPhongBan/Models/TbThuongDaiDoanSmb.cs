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

    [ForeignKey(nameof(IdTongThuongSmb))]
    public TbThuongSmb? TbThuongSmb { get; set; }

    public virtual TbPhongBan? IdPhongBanNavigation { get; set; }
}
