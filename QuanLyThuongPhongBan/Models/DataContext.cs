using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models.Entities;

namespace QuanLyThuongPhongBan.Data;

/// <summary>
/// DbContext chính cho ứng dụng quản lý thưởng doanh thu
/// Sử dụng Entity Framework Core với SQL Server
/// </summary>
public class DataContext : DbContext
{
    // Constructor parameterless cho design-time và SQLite
    public DataContext() { }

    // Constructor với options cho dependency injection
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    // Các DbSet với tên tiếng Anh
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<ProjectTeamBonus> ProjectTeamBonuses { get; set; }
    public DbSet<SmbTeamBonus> SmbTeamBonuses { get; set; }
    public DbSet<ProjectBonus> ProjectBonuses { get; set; }
    public DbSet<ProjectBonusDetail> ProjectBonusDetails { get; set; }
    public DbSet<SmbBonus> SmbBonuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureActivityLog(modelBuilder);
        ConfigureDepartment(modelBuilder);
        ConfigureAccount(modelBuilder);
        ConfigureProjectTeamBonus(modelBuilder);
        ConfigureSmbTeamBonus(modelBuilder);
        ConfigureProjectBonus(modelBuilder);
        ConfigureProjectBonusDetail(modelBuilder);
        ConfigureSmbBonus(modelBuilder);
    }

    /// <summary>
    /// Cấu hình bảng Nhật ký hoạt động (tb_nhat_ky)
    /// </summary>
    private void ConfigureActivityLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.ToTable("tb_nhat_ky");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.AccountId)
                .HasColumnName("id_tai_khoan")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Action)
                .HasColumnName("hanh_dong")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("mo_ta")
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAddOrUpdate();

            // Relationship với Account
            entity.HasOne(e => e.Account)
                  .WithMany(a => a.ActivityLogs)
                  .HasForeignKey(e => e.AccountId)
                  .HasPrincipalKey(a => a.Username)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    /// <summary>
    /// Cấu hình bảng Phòng ban (tb_phong_ban)
    /// </summary>
    private void ConfigureDepartment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("tb_phong_ban");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Name)
                .HasColumnName("ten_phong_ban")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("mo_ta")
                .HasMaxLength(255);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAddOrUpdate();

            // Index cho tên phòng ban
            entity.HasIndex(e => e.Name)
                  .IsUnique();
        });
    }

    /// <summary>
    /// Cấu hình bảng Tài khoản (tb_tai_khoan)
    /// </summary>
    private void ConfigureAccount(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("tb_tai_khoan");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Username)
                .HasColumnName("ten_dang_nhap")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Password)
                .HasColumnName("mat_khau")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.FullName)
                .HasColumnName("ho_ten")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.DepartmentId)
                .HasColumnName("id_phong_ban");

            // Unique constraint cho username
            entity.HasIndex(e => e.Username)
                  .IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAddOrUpdate();

            // Relationship với Department
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Accounts)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    /// <summary>
    /// Cấu hình bảng Thưởng đại đoàn dự án (tb_thuong_dai_doan_du_an)
    /// </summary>
    private void ConfigureProjectTeamBonus(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectTeamBonus>(entity =>
        {
            entity.ToTable("tb_thuong_dai_doan_du_an");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.DepartmentId)
                .HasColumnName("id_phong_ban");

            entity.Property(e => e.ProjectBonusId)
                .HasColumnName("id_thuong_du_an");

            // Các cột giá trị và tỷ lệ
            entity.Property(e => e.TotalPackageValue)
                .HasColumnName("gia_tri_tong_goi")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalPackageRate)
                .HasColumnName("ti_le_tong_goi")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.Adjustment1Value)
                .HasColumnName("gia_tri_dieu_chinh_dot_1")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.Adjustment1Rate)
                .HasColumnName("ti_le_dieu_chinh_dot_1")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.Adjustment2Value)
                .HasColumnName("gia_tri_dieu_chinh_dot_2")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.Adjustment2Rate)
                .HasColumnName("ti_le_dieu_chinh_dot_2")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.Acceptance)
                .HasColumnName("nghiem_thu")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.DebtRecovery)
                .HasColumnName("thu_hoi_cong_no")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.InvoiceRevenue)
                .HasColumnName("doanh_thu_xuat_hoa_don")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.ContractRevenue)
                .HasColumnName("doanh_thu_hop_dong")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAddOrUpdate();

            // Relationships
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.ProjectTeamBonuses)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProjectBonus)
                  .WithMany(p => p.ProjectTeamBonuses)
                  .HasForeignKey(e => e.ProjectBonusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    /// <summary>
    /// Cấu hình bảng Thưởng đại đoàn SMB (tb_thuong_dai_doan_smb)
    /// </summary>
    private void ConfigureSmbTeamBonus(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SmbTeamBonus>(entity =>
        {
            entity.ToTable("tb_thuong_dai_doan_smb");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.DepartmentId)
                .HasColumnName("id_phong_ban");

            entity.Property(e => e.SmbBonusId)
                .HasColumnName("id_thuong_smb");
            
            entity.Property(e => e.Phase1Value)
                .HasColumnName("gia_tri_dot_1")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.Phase1Rate)
                .HasColumnName("ti_le_dot_1")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalSmbValue)
                .HasColumnName("gia_tri_tong_smb")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalSmbRate)
                .HasColumnName("ti_le_tong_smb")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.Acceptance)
                .HasColumnName("nghiem_thu")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.DebtRecovery)
                .HasColumnName("thu_hoi_cong_no")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.DebtRecoveryRate)
                .HasColumnName("ti_le_thu_hoi_cong_no")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.InvoiceRevenue)
                .HasColumnName("doanh_thu_xuat_hoa_don")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.ContractRevenue)
                .HasColumnName("doanh_thu_hop_dong")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAddOrUpdate();

            // Relationships
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.SmbTeamBonuses)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SmbBonus)
                  .WithMany(s => s.SmbTeamBonuses)
                  .HasForeignKey(e => e.SmbBonusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    /// <summary>
    /// Cấu hình bảng Thưởng dự án (tb_thuong_du_an)
    /// </summary>
    private void ConfigureProjectBonus(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectBonus>(entity =>
        {
            entity.ToTable("tb_thuong_du_an");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Year)
                .HasColumnName("nam_thuong")
                .HasMaxLength(4);

            entity.Property(e => e.ContractValue)
                .HasColumnName("gia_tri_hop_dong")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.Settlement)
                .HasColumnName("quyet_toan")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            // Các cột tổng
            entity.Property(e => e.TotalAdjustment1Value)
                .HasColumnName("tong_gia_tri_dieu_chinh_dot_1")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalAdjustment2Value)
                .HasColumnName("tong_gia_tri_dieu_chinh_dot_2")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalAdjustment1Rate)
                .HasColumnName("tong_ti_le_dieu_chinh_dot_1")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalAdjustment2Rate)
                .HasColumnName("tong_ti_le_dieu_chinh_dot_2")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalAcceptance)
                .HasColumnName("tong_nghiem_thu")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalDebtRecovery)
                .HasColumnName("tong_thu_hoi_cong_no")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalProjectBonusValue)
                .HasColumnName("tong_gia_tri_thuong_du_an")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalProjectBonusRate)
                .HasColumnName("tong_ti_le_thuong_du_an")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAddOrUpdate();
        });
    }

    /// <summary>
    /// Cấu hình bảng Thưởng dự án chi tiết (tb_thuong_du_an_chi_tiet)
    /// </summary>
    private void ConfigureProjectBonusDetail(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectBonusDetail>(entity =>
        {
            entity.ToTable("tb_thuong_du_an_chi_tiet");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.ProjectBonusId)
                .HasColumnName("id_thuong_du_an");

            entity.Property(e => e.ProjectName)
                .HasColumnName("du_an")
                .HasMaxLength(255);

            entity.Property(e => e.Investor)
                .HasColumnName("chu_dau_tu")
                .HasMaxLength(255);

            entity.Property(e => e.ContractNumber)
                .HasColumnName("hop_dong_so")
                .HasMaxLength(100);

            entity.Property(e => e.Date)
                .HasColumnName("ngay_thang");

            entity.Property(e => e.Notes)
                .HasColumnName("ghi_chu")
                .HasMaxLength(500);

            // Các cột doanh thu
            entity.Property(e => e.ContractRevenue)
                .HasColumnName("doanh_thu_hop_dong")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.SettlementRevenue)
                .HasColumnName("doanh_thu_quyet_toan")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.InvoicedRevenue)
                .HasColumnName("doanh_thu_da_xuat_hoa_don")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.UninvoicedRevenue)
                .HasColumnName("doanh_thu_chua_xuat_hoa_don")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.PaidAmount)
                .HasColumnName("da_thanh_toan")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.UnpaidAmount)
                .HasColumnName("chua_thanh_toan")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            // Các cột hệ số
            entity.Property(e => e.HSDA)
                .HasColumnName("hsda")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.KTK)
                .HasColumnName("ktk")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.PO)
                .HasColumnName("po")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TTDVKT)
                .HasColumnName("ttdvkt")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TVGP)
                .HasColumnName("tvgp")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAddOrUpdate();

            // Relationship với ProjectBonus
            entity.HasOne(e => e.ProjectBonus)
                  .WithMany(p => p.ProjectBonusDetails)
                  .HasForeignKey(e => e.ProjectBonusId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Cấu hình bảng Thưởng SMB (tb_thuong_smb)
    /// </summary>
    private void ConfigureSmbBonus(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SmbBonus>(entity =>
        {
            entity.ToTable("tb_thuong_smb");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.QuarterYear)
                .HasColumnName("quy_nam_thuong")
                .HasMaxLength(50);

            // Các cột tổng
            entity.Property(e => e.TotalPhase1Value)
                .HasColumnName("tong_gia_tri_dot_1")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalPhase1Rate)
                .HasColumnName("tong_ti_le_dot_1")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalSmbValue)
                .HasColumnName("tong_gia_tri_smb")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalAcceptance)
                .HasColumnName("tong_nghiem_thu")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalDebtRecovery)
                .HasColumnName("tong_thu_hoi_cong_no")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalSmbBonusValue)
                .HasColumnName("tong_gia_tri_thuong_smb")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.TotalSmbBonusRate)
                .HasColumnName("tong_ti_le_thuong_smb")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.InvoiceOutput)
                .HasColumnName("xuat_hoa_don")
                .HasColumnType("decimal(18, 6)")
                .HasDefaultValue(0m);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAddOrUpdate();
        });
    }
}