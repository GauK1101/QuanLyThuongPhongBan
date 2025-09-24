using QuanLyThuongPhongBan.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongDaiDoanDuAn : BaseViewModel
{
    public int Id { get => Get<int>(); set => Set(value); }

    [ForeignKey("IdPhongBanNavigation")]
    public int IdPhongBan { get => Get<int>(); set => Set(value); }

    public int IdThuongDuAnPhongBan { get => Get<int>(); set => Set(value); }

    public decimal TiLeThuong { get => Get<decimal>(); set => Set(value); }

    public decimal GiaTri { get => Get<decimal>(); set => Set(value); }

    public decimal GiaTriDieuChinhDot1 { get => Get<decimal>(); set => Set(value); }

    public decimal TiLeDieuChinhDot1 { get => Get<decimal>(); set => Set(value); }

    public decimal GiaTriDieuChinhDot2 { get => Get<decimal>(); set => Set(value); }

    public decimal TiLeDieuChinhDot2 { get => Get<decimal>(); set => Set(value); }

    public decimal ThuHoiCongNo { get => Get<decimal>(); set => Set(value); }

    public decimal NghiemThu { get => Get<decimal>(); set => Set(value); }

    [ForeignKey(nameof(IdThuongDuAnPhongBan))]
    public TbThuongDuAn? TbThuongDuAn { get; set; }

    public virtual TbPhongBan? IdPhongBanNavigation { get; set; }
}
