using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyThuongPhongBan.Models;

public partial class QuanLyThuongDoanhThuContext : DbContext
{
    public QuanLyThuongDoanhThuContext()
    {
    }

    public QuanLyThuongDoanhThuContext(DbContextOptions<QuanLyThuongDoanhThuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbNhatKy> TbNhatKies { get; set; }

    public virtual DbSet<TbPhongBan> TbPhongBans { get; set; }

    public virtual DbSet<TbTaiKhoan> TbTaiKhoans { get; set; }

    public virtual DbSet<TbThuHoiCongNoSmbTheoThang> TbThuHoiCongNoSmbTheoThangs { get; set; }

    public virtual DbSet<TbThuongDaiDoanDuAn> TbThuongDaiDoanDuAns { get; set; }

    public virtual DbSet<TbThuongDaiDoanSmb> TbThuongDaiDoanSmbs { get; set; }

    public virtual DbSet<TbThuongDuAn> TbThuongDuAns { get; set; }

    public virtual DbSet<TbThuongDuAnChiTiet> TbThuongDuAnChiTiets { get; set; }

    public virtual DbSet<TbThuongSmb> TbThuongSmbs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=192.168.1.110;Initial Catalog=quan_ly_thuong_doanh_thu;Persist Security Info=True;User ID=user;Password=KTD@8888;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbNhatKy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_nhat___3214EC074458233A");

            entity.ToTable("tb_nhat_ky");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.HanhDong)
                .HasMaxLength(200)
                .HasColumnName("hanh_dong");
            entity.Property(e => e.IdTaiKhoan)
                .HasMaxLength(50)
                .HasColumnName("id_tai_khoan");
            entity.Property(e => e.MoTa)
                .HasMaxLength(500)
                .HasColumnName("mo_ta");
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("thoi_gian");
        });

        modelBuilder.Entity<TbPhongBan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_phong__3213E83F0C4DE340");

            entity.ToTable("tb_phong_ban");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MoTa)
                .HasMaxLength(255)
                .HasColumnName("mo_ta");
            entity.Property(e => e.TenPhongBan)
                .HasMaxLength(100)
                .HasColumnName("ten_phong_ban");
        });

        modelBuilder.Entity<TbTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_tai_k__3214EC0762D837CA");

            entity.ToTable("tb_tai_khoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__tb_tai_k__55F68FC01D5CB022").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .HasColumnName("ho_ten");
            entity.Property(e => e.IdPhongBan).HasColumnName("id_phong_ban");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .HasColumnName("mat_khau");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .HasColumnName("ten_dang_nhap");
        });

        modelBuilder.Entity<TbThuHoiCongNoSmbTheoThang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thu_h__3214EC0710BF7739");

            entity.ToTable("tb_thu_hoi_cong_no_smb_theo_thang");

            entity.Property(e => e.Thang1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang10)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang11)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang12)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang2)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang3)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang4)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang5)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang6)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang7)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang8)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Thang9)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)");
        });

        modelBuilder.Entity<TbThuongDaiDoanDuAn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83F0434E8BF");

            entity.ToTable("tb_thuong_dai_doan_du_an");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTriDieuChinhDot1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_dieu_chinh_dot_1");
            entity.Property(e => e.GiaTriDieuChinhDot2)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_dieu_chinh_dot_2");
            entity.Property(e => e.GiaTriTongGoi)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_tong_goi");
            entity.Property(e => e.IdPhongBan).HasColumnName("id_phong_ban");
            entity.Property(e => e.IdThuongDuAn).HasColumnName("id_thuong_du_an");
            entity.Property(e => e.NghiemThu)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("nghiem_thu");
            entity.Property(e => e.ThuHoiCongNo)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("thu_hoi_cong_no");
            entity.Property(e => e.TiLeDieuChinhDot1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_dieu_chinh_dot_1");
            entity.Property(e => e.TiLeDieuChinhDot2)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_dieu_chinh_dot_2");
            entity.Property(e => e.TiLeTongGoi)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_tong_goi");
        });

        modelBuilder.Entity<TbThuongDaiDoanSmb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83FB31CD93A");

            entity.ToTable("tb_thuong_dai_doan_smb");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTriDot1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_dot_1");
            entity.Property(e => e.GiaTriTongSmb)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_tong_smb");
            entity.Property(e => e.IdPhongBan).HasColumnName("id_phong_ban");
            entity.Property(e => e.IdThuongSmb).HasColumnName("id_thuong_smb");
            entity.Property(e => e.NghiemThu)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("nghiem_thu");
            entity.Property(e => e.ThuHoiCongNo)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("thu_hoi_cong_no");
            entity.Property(e => e.TiLeDot1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_dot_1");
            entity.Property(e => e.TiLeThuHoiCongNo)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_thu_hoi_cong_no");
            entity.Property(e => e.TiLeTongSmb)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_tong_smb");
        });

        modelBuilder.Entity<TbThuongDuAn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83F345FE0B4");

            entity.ToTable("tb_thuong_du_an");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTriHopDong)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_hop_dong");
            entity.Property(e => e.NamThuong)
                .HasMaxLength(4)
                .HasColumnName("nam_thuong");
            entity.Property(e => e.QuyetToan)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("quyet_toan");
            entity.Property(e => e.TongGiaTriDieuChinhDot1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_dieu_chinh_dot_1");
            entity.Property(e => e.TongGiaTriDieuChinhDot2)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_dieu_chinh_dot_2");
            entity.Property(e => e.TongGiaTriThuongDuAn)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_thuong_du_an");
            entity.Property(e => e.TongNghiemThu)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_nghiem_thu");
            entity.Property(e => e.TongThuHoiCongNo)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_thu_hoi_cong_no");
            entity.Property(e => e.TongTiLeDieuChinhDot1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_dieu_chinh_dot_1");
            entity.Property(e => e.TongTiLeDieuChinhDot2)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_dieu_chinh_dot_2");
            entity.Property(e => e.TongTiLeThuongDuAn)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_thuong_du_an");
        });

        modelBuilder.Entity<TbThuongDuAnChiTiet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83F61FFE193");

            entity.ToTable("tb_thuong_du_an_chi_tiet");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdThuongDuAn).HasColumnName("id_thuong_du_an");
            entity.Property(e => e.ChuDauTu)
                .HasMaxLength(255)
                .HasColumnName("chu_dau_tu");
            entity.Property(e => e.DaThanhToan)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("da_thanh_toan");
            entity.Property(e => e.ChuaThanhToan)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("chua_thanh_toan");
            entity.Property(e => e.DoanhThuChuaXuatHoaDon)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("doanh_thu_chua_xuat_hoa_don");
            entity.Property(e => e.DoanhThuDaXuatHoaDon)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("doanh_thu_da_xuat_hoa_don");
            entity.Property(e => e.DoanhThuHopDong)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("doanh_thu_hop_dong");
            entity.Property(e => e.DoanhThuQuyetToan)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("doanh_thu_quyet_toan");
            entity.Property(e => e.DuAn)
                .HasMaxLength(255)
                .HasColumnName("du_an");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasColumnName("ghi_chu");
            entity.Property(e => e.HopDongSo)
                .HasMaxLength(100)
                .HasColumnName("hop_dong_so");
            entity.Property(e => e.Hsda)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("hsda");
            entity.Property(e => e.Ktk)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ktk");
            entity.Property(e => e.NgayThang).HasColumnName("ngay_thang");
            entity.Property(e => e.Po)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("po");
            entity.Property(e => e.Ttdvkt)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ttdvkt");
            entity.Property(e => e.Tvgp)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tvgp");
        });

        modelBuilder.Entity<TbThuongSmb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83F3B62B3F7");

            entity.ToTable("tb_thuong_smb");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.QuyNamThuong)
                .HasMaxLength(50)
                .HasColumnName("quy_nam_thuong");
            entity.Property(e => e.TongGiaTriDot1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_dot_1");
            entity.Property(e => e.TongGiaTriSmb)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_smb");
            entity.Property(e => e.TongGiaTriThuongSmb)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_thuong_smb");
            entity.Property(e => e.TongNghiemThu)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_nghiem_thu");
            entity.Property(e => e.TongThuHoiCongNo)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_thu_hoi_cong_no");
            entity.Property(e => e.TongTiLeDot1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_dot_1");
            entity.Property(e => e.TongTiLeThuongSmb)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_thuong_smb");
            entity.Property(e => e.XuatHoaDon)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("xuat_hoa_don");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
