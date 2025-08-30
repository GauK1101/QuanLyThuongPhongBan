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

    public virtual DbSet<TbPhongBan> TbPhongBans { get; set; }

    public virtual DbSet<TbThuongDaiDoanDuAn> TbThuongDaiDoanDuAns { get; set; }

    public virtual DbSet<TbThuongDaiDoanSmb> TbThuongDaiDoanSmbs { get; set; }

    public virtual DbSet<TbThuongDuAn> TbThuongDuAns { get; set; }

    public virtual DbSet<TbThuongSmb> TbThuongSmbs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=192.168.1.110;Initial Catalog=quan_ly_thuong_doanh_thu;Persist Security Info=True;User ID=user;Password=KTD@8888;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbPhongBan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_phong__3213E83F43640A19");

            entity.ToTable("tb_phong_ban");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MoTa)
                .HasMaxLength(255)
                .HasColumnName("mo_ta");
            entity.Property(e => e.TenPhongBan)
                .HasMaxLength(100)
                .HasColumnName("ten_phong_ban");
        });

        modelBuilder.Entity<TbThuongDaiDoanDuAn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83FA9F7769A");

            entity.ToTable("tb_thuong_dai_doan_du_an");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTri)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gia_tri");
            entity.Property(e => e.GiaTriDieuChinhDot1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gia_tri_dieu_chinh_dot_1");
            entity.Property(e => e.GiaTriDieuChinhDot2)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gia_tri_dieu_chinh_dot_2");
            entity.Property(e => e.IdPhongBan).HasColumnName("id_phong_ban");
            entity.Property(e => e.IdThuongDuAnPhongBan).HasColumnName("id_thuong_du_an_phong_ban");
            entity.Property(e => e.NghiemThu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nghiem_thu");
            entity.Property(e => e.ThuHoiCongNo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("thu_hoi_cong_no");
            entity.Property(e => e.TiLeDieuChinhDot1)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("ti_le_dieu_chinh_dot_1");
            entity.Property(e => e.TiLeDieuChinhDot2)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("ti_le_dieu_chinh_dot_2");
            entity.Property(e => e.TiLeThuong)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("ti_le_thuong");
        });

        modelBuilder.Entity<TbThuongDaiDoanSmb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83F435DD103");

            entity.ToTable("tb_thuong_dai_doan_smb");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTriDot1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gia_tri_dot_1");
            entity.Property(e => e.IdPhongBan).HasColumnName("id_phong_ban");
            entity.Property(e => e.IdTongThuongSmb).HasColumnName("id_tong_thuong_smb");
            entity.Property(e => e.NghiemThu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nghiem_thu");
            entity.Property(e => e.ThuHoiCongNo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("thu_hoi_cong_no");
            entity.Property(e => e.TiLeDot1)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("ti_le_dot_1");
            entity.Property(e => e.TiLeGiaTri)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("ti_le_gia_tri");
            entity.Property(e => e.TiLeSmb)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("ti_le_smb");
        });

        modelBuilder.Entity<TbThuongDuAn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83FBFC0C305");

            entity.ToTable("tb_thuong_du_an");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTriHopDong)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("gia_tri_hop_dong");
            entity.Property(e => e.NamThuong)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nam_thuong");
            entity.Property(e => e.QuyetToan)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("quyet_toan");
            entity.Property(e => e.TiLeThuongPhongBan)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("ti_le_thuong_phong_ban");
            entity.Property(e => e.TongGiaTriThuongPhongBan)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tong_gia_tri_thuong_phong_ban");
        });

        modelBuilder.Entity<TbThuongSmb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83FF01B77F7");

            entity.ToTable("tb_thuong_smb");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TiLeQuyetToan)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("ti_le_quyet_toan");
            entity.Property(e => e.TongGiaTriSmb)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tong_gia_tri_smb");
            entity.Property(e => e.TongGiaTriSmbDieuChinh)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("tong_gia_tri_smb_dieu_chinh");
            entity.Property(e => e.TongTiLeSmb)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("tong_ti_le_smb");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
