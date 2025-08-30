using System;
using System.Collections.Generic;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongSmb
{
    public int Id { get; set; }

    public decimal TongGiaTriSmb { get; set; }

    public decimal TiLeQuyetToan { get; set; }

    public decimal TongTiLeSmb { get; set; }

    public decimal TongGiaTriSmbDieuChinh { get; set; }
}
