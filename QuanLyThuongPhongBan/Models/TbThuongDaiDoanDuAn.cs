using System;
using System.Collections.Generic;

namespace QuanLyThuongPhongBan.Models;

public partial class TbThuongDaiDoanDuAn
{
    public int Id { get; set; }

    public int IdPhongBan { get; set; }

    public int IdThuongDuAnPhongBan { get; set; }

    public decimal TiLeThuong { get; set; }

    public decimal GiaTri { get; set; }

    public decimal GiaTriDieuChinhDot1 { get; set; }

    public decimal TiLeDieuChinhDot1 { get; set; }

    public decimal GiaTriDieuChinhDot2 { get; set; }

    public decimal TiLeDieuChinhDot2 { get; set; }

    public decimal ThuHoiCongNo { get; set; }

    public decimal NghiemThu { get; set; }
}
