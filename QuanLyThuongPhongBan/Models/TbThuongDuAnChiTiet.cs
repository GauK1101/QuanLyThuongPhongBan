using QuanLyThuongPhongBan.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongDuAnChiTiet : BaseViewModel
{
    public int Id
    {
        get => Get<int>();
        set => Set(value);
    }

    public int IdThuongDuAn
    {
        get => Get<int>();
        set => Set(value);
    }

    public string? ChuDauTu
    {
        get => Get<string?>();
        set => Set(value);
    }

    public string? HopDongSo
    {
        get => Get<string?>();
        set => Set(value);
    }

    public string? DuAn
    {
        get => Get<string?>();
        set => Set(value);
    }

    public DateOnly? NgayThang
    {
        get => Get<DateOnly?>();
        set => Set(value);
    }

    public decimal? DoanhThuHopDong
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? DoanhThuQuyetToan
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? DoanhThuDaXuatHoaDon
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? DoanhThuChuaXuatHoaDon
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? DaThanhToan
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public string? GhiChu
    {
        get => Get<string?>();
        set => Set(value);
    }

    public decimal? Tvgp
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? Hsda
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? Po
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? Ktk
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? Ttdvkt
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    [ForeignKey(nameof(IdThuongDuAn))]
    public TbThuongDuAn? TbThuongDuAn { get; set; }
}
