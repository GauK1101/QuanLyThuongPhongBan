using System;
using System.Collections.Generic;

namespace QuanLyThuongPhongBan.Models;

public partial class TbPhongBan
{
    public int Id { get; set; }

    public string TenPhongBan { get; set; } = null!;

    public string MoTa { get; set; } = null!;
}
