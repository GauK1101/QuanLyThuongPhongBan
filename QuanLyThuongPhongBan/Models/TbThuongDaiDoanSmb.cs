using System;
using System.Collections.Generic;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongDaiDoanSmb
{
    public int Id { get; set; }

    public int IdPhongBan { get; set; }

    public int IdTongThuongSmb { get; set; }

    public decimal TiLeSmb { get; set; }

    public decimal TiLeGiaTri { get; set; }

    public decimal TiLeDot1 { get; set; }

    public decimal GiaTriDot1 { get; set; }

    public decimal ThuHoiCongNo { get; set; }

    public decimal NghiemThu { get; set; }
}
