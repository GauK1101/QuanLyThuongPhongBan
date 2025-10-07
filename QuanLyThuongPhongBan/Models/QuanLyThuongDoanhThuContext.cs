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

    public virtual DbSet<TbThuongDaiDoanDuAn> TbThuongDaiDoanDuAns { get; set; }

    public virtual DbSet<TbThuongDaiDoanSmb> TbThuongDaiDoanSmbs { get; set; }

    public virtual DbSet<TbThuongDuAn> TbThuongDuAns { get; set; }

    public virtual DbSet<TbThuongSmb> TbThuongSmbs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=192.168.1.110;Initial Catalog=quan_ly_thuong_doanh_thu;Persist Security Info=True;User ID=user;Password=KTD@8888;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbNhatKy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_nhat___3214EC074458233A");

            entity.ToTable("tb_nhat_ky");

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.IdTaiKhoan)
                .HasColumnName("id_tai_khoan")
                .HasMaxLength(50);
            entity.Property(e => e.MoTa)
                .HasColumnName("mo_ta")
                .HasMaxLength(500);
            entity.Property(e => e.HanhDong)
                .HasColumnName("hanh_dong")
                .HasMaxLength(200);
            entity.Property(e => e.ThoiGian)
                .HasColumnName("thoi_gian")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");
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

            entity.HasIndex(e => e.TenDangNhap, "UQ__tb_tai_k__55F68FC01D5CB022")
                .IsUnique();
            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.HoTen)
                .HasColumnName("ho_ten")
                .HasMaxLength(100);
            entity.Property(e => e.MatKhau)
                .HasColumnName("mat_khau")
                .HasMaxLength(255);
            entity.Property(e => e.TenDangNhap)
                .HasColumnName("ten_dang_nhap")
                .HasMaxLength(50);
            entity.Property(e => e.IdPhongBan)
                .HasColumnName("id_phong_ban");
        });


        modelBuilder.Entity<TbThuongDaiDoanDuAn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83F0434E8BF");

            entity.ToTable("tb_thuong_dai_doan_du_an");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTriDieuChinhDot1)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_dieu_chinh_dot_1");
            entity.Property(e => e.GiaTriDieuChinhDot2)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_dieu_chinh_dot_2");
            entity.Property(e => e.GiaTriTongGoi)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_tong_goi");
            entity.Property(e => e.IdPhongBan).HasColumnName("id_phong_ban");
            entity.Property(e => e.IdThuongDuAn).HasColumnName("id_thuong_du_an");
            entity.Property(e => e.NghiemThu)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("nghiem_thu");
            entity.Property(e => e.ThuHoiCongNo)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("thu_hoi_cong_no");
            entity.Property(e => e.TiLeDieuChinhDot1)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_dieu_chinh_dot_1");
            entity.Property(e => e.TiLeDieuChinhDot2)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_dieu_chinh_dot_2");
            entity.Property(e => e.TiLeTongGoi)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_tong_goi");
        });

        modelBuilder.Entity<TbThuongDaiDoanSmb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83FB31CD93A");

            entity.ToTable("tb_thuong_dai_doan_smb");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTriDot1)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_dot_1");
            entity.Property(e => e.GiaTriTongSmb)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_tong_smb");
            entity.Property(e => e.IdPhongBan).HasColumnName("id_phong_ban");
            entity.Property(e => e.IdThuongSmb).HasColumnName("id_thuong_smb");
            entity.Property(e => e.NghiemThu)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("nghiem_thu");
            entity.Property(e => e.DaThuHoiCongNo)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("da_thu_hoi_cong_no");
            entity.Property(e => e.ChuaThuHoiCongNo)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("chua_thu_hoi_cong_no");
            entity.Property(e => e.TiLeDot1)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_dot_1");
            entity.Property(e => e.TiLeTongSmb)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("ti_le_tong_smb");
        });

        modelBuilder.Entity<TbThuongDuAn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83F345FE0B4");

            entity.ToTable("tb_thuong_du_an");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTriHopDong)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gia_tri_hop_dong");
            entity.Property(e => e.NamThuong)
                .HasMaxLength(4)
                .HasColumnName("nam_thuong");
            entity.Property(e => e.QuyetToan)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("quyet_toan");
            entity.Property(e => e.TongGiaTriThuongDuAn)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_thuong_du_an");
            entity.Property(e => e.TongTiLeThuongDuAn)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_thuong_du_an");
            entity.Property(e => e.TongGiaTriDieuChinhDot1)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_dieu_chinh_dot_1");
            entity.Property(e => e.TongTiLeDieuChinhDot1)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_dieu_chinh_dot_1");
            entity.Property(e => e.TongGiaTriDieuChinhDot2)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_dieu_chinh_dot_2");
            entity.Property(e => e.TongTiLeDieuChinhDot2)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_dieu_chinh_dot_2");
            entity.Property(e => e.TongThuHoiCongNo)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_thu_hoi_cong_no");
            entity.Property(e => e.TongNghiemThu)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_nghiem_thu");
        });

        modelBuilder.Entity<TbThuongSmb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tb_thuon__3213E83F3B62B3F7");

            entity.ToTable("tb_thuong_smb");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NamThuong)
                .HasMaxLength(4)
                .HasColumnName("nam_thuong");
            entity.Property(e => e.TongGiaTriSmb)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_smb");
            entity.Property(e => e.TongGiaTriThuongSmb)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_thuong_smb");
            entity.Property(e => e.TongTiLeThuongSmb)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_thuong_smb");
            entity.Property(e => e.XuatHoaDon)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("xuat_hoa_don");
            entity.Property(e => e.TongTiLeDot1)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_ti_le_dot_1");
            entity.Property(e => e.TongGiaTriDot1)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_gia_tri_dot_1");
            entity.Property(e => e.TongThuHoiCongNo)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_thu_hoi_cong_no");
            entity.Property(e => e.TongNghiemThu)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("tong_nghiem_thu");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
