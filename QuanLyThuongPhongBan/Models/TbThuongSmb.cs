using QuanLyThuongPhongBan.ViewModels;
using System.Collections.ObjectModel;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongSmb : BaseViewModel
{
    public int Id
    {
        get => Get<int>();
        set => Set(value);
    }

    public decimal? TongGiaTriSmb
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? XuatHoaDon
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? TongTiLeThuongSmb
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? TongGiaTriThuongSmb
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public string? NamThuong
    {
        get => Get<string?>();
        set => Set(value);
    }

    public decimal? TongTiLeDot1
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? TongGiaTriDot1
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? TongThuHoiCongNo
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public decimal? TongNghiemThu
    {
        get => Get<decimal?>();
        set => Set(value);
    }

    public ObservableCollection<TbThuongDaiDoanSmb> Details
    {
        get
        {
            var v = Get<ObservableCollection<TbThuongDaiDoanSmb>>();
            if (v == null)
            {
                v = new ObservableCollection<TbThuongDaiDoanSmb>();
                Set(v); // QUAN TRỌNG: lưu lại, để các lần sau không tạo mới
            }
            return v;
        }
        set => Set(value);
    }
}
