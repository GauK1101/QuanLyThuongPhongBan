using System;
using System.Collections.Generic;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongDuAn
{
    public int Id { get; set; }

    public decimal GiaTriHopDong { get; set; }

    public decimal QuyetToan { get; set; }

    public decimal TiLeThuongPhongBan { get; set; }

    public decimal TongGiaTriThuongPhongBan { get; set; }

    public string NamThuong { get; set; } = null!;

    public List<TbThuongDaiDoanDuAn> Details { get; set; } = new List<TbThuongDaiDoanDuAn>();
}
