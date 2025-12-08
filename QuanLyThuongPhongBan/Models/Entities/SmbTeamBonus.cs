using QuanLyThuongPhongBan.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tb_thuong_dai_doan_smb")]
[Display(Name = "📋 Chi tiết SMB")]
public class SmbTeamBonus
{
    [Key]
    [Column("id")]
    [Display(Name = "🆔 ID")]
    public int Id { get; set; }

    /// <summary>
    /// ID phòng ban
    /// </summary>
    [Display(Name = "🏢 Phòng ban")]
    [Column("id_phong_ban")]
    public int DepartmentId { get; set; }

    [Column("id_thuong_smb")]
    public int SmbBonusId { get; set; }

    /// <summary>
    /// Doanh thu smb
    /// </summary>
    [Display(Name = "📑 Doanh thu Smb")]
    [Column("doanh_thu_smb", TypeName = "decimal(22, 6)")]
    public decimal SmbRevenue { get; set; }

    /// <summary>
    /// Doanh thu xuất hóa đơn
    /// </summary>
    [Display(Name = "🧾 Doanh thu xuất hóa đơn")]
    [Column("doanh_thu_xuat_hoa_don", TypeName = "decimal(22, 6)")]
    public decimal InvoiceRevenue { get; set; }

    /// <summary>
    /// Thu hồi công nợ
    /// </summary>
    [Display(Name = "🔄 Doanh thu thu hồi công nợ")]
    [Column("doanh_thu_thu_hoi_cong_no", TypeName = "decimal(22, 6)")]
    public decimal DebtRecoveryRevenue { get; set; }

    /// <summary>
    /// Tỷ lệ đợt 1
    /// </summary>
    [Display(Name = "📊 Tỷ lệ đợt 1")]
    [Column("ti_le_dot_1", TypeName = "decimal(22, 6)")]
    public decimal Phase1Rate { get; set; }

    /// <summary>
    /// Giá trị đợt 1
    /// </summary>
    [Display(Name = "💰 Giá trị đợt 1")]
    [Column("gia_tri_dot_1", TypeName = "decimal(22, 6)")]
    public decimal Phase1Value { get; set; }

    /// <summary>
    /// Tỷ lệ tổng SMB
    /// </summary>
    [Display(Name = "📈 Tỷ lệ tổng SMB")]
    [Column("ti_le_tong_smb", TypeName = "decimal(22, 6)")]
    public decimal TotalSmbRate { get; set; }

    /// <summary>
    /// Giá trị tổng SMB
    /// </summary>
    [Display(Name = "💵 Giá trị tổng SMB")]
    [Column("gia_tri_tong_smb", TypeName = "decimal(22, 6)")]
    public decimal TotalSmbValue { get; set; }

    /// <summary>
    /// Tỷ lệ thu hồi công nợ
    /// </summary>
    [Display(Name = "📊 Tỷ lệ thu hồi công nợ")]
    [Column("ti_le_thu_hoi_cong_no", TypeName = "decimal(22, 6)")]
    public decimal DebtRecoveryRate { get; set; }

    /// <summary>
    /// Thu hồi công nợ
    /// </summary>
    [Display(Name = "🔄 Thu hồi công nợ")]
    [Column("thu_hoi_cong_no", TypeName = "decimal(22, 6)")]
    public decimal DebtRecovery { get; set; }

    /// <summary>
    /// Nghiệm thu
    /// </summary>
    [Display(Name = "✅ Nghiệm thu")]
    [Column("nghiem_thu", TypeName = "decimal(22, 6)")]
    public decimal Acceptance { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("DepartmentId")]
    public virtual Department Department { get; set; } = null!;

    [ForeignKey("SmbBonusId")]
    public virtual SmbBonus SmbBonus { get; set; } = null!;
}