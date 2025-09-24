using QuanLyThuongPhongBan.ViewModels;
using System.Collections.ObjectModel;
using System.Reflection;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongDuAn : BaseViewModel
{
    public int Id
    {
        get => Get<int>();
        set => Set(value);
    }

    public decimal GiaTriHopDong
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal QuyetToan
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal TiLeThuongPhongBan
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public decimal TongGiaTriThuongPhongBan
    {
        get => Get<decimal>();
        set => Set(value);
    }

    public string NamThuong
    {
        get => Get<string>() ?? string.Empty;
        set => Set(value);
    }

    public ObservableCollection<TbThuongDaiDoanDuAn> Details
    {
        get
        {
            var v = Get<ObservableCollection<TbThuongDaiDoanDuAn>>();
            if (v == null)
            {
                v = new ObservableCollection<TbThuongDaiDoanDuAn>();
                Set(v); // QUAN TRỌNG: lưu lại, để các lần sau không tạo mới
            }
            return v;
        }
        set => Set(value);
    }
}
