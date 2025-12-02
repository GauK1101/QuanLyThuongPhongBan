using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("tb_thuong_smb")]
[Display(Name = "🎁 Thưởng SMB")]
public class SmbBonus
{
    [Key]
    [Column("id")]
    [Display(Name = "🆔 ID")]
    public int Id { get; set; }

    /// <summary>
    /// Quý năm thưởng
    /// </summary>
    [Display(Name = "📅 Quý năm thưởng")]
    [Column("quy_nam_thuong")]
    [StringLength(50)]
    public string QuarterYear { get; set; } = string.Empty;

    /// <summary>
    /// Tổng giá trị thưởng SMB
    /// </summary>
    [Display(Name = "🎁 Tổng giá trị thưởng SMB")]
    [Column("tong_gia_tri_thuong_smb", TypeName = "decimal(18, 6)")]
    public decimal TotalSmbBonusValue { get; set; }

    /// <summary>
    /// Xuất hóa đơn
    /// </summary>
    [Display(Name = "🧾 Xuất hóa đơn")]
    [Column("xuat_hoa_don", TypeName = "decimal(18, 6)")]
    public decimal InvoiceOutput { get; set; }

    /// <summary>
    /// Tổng tỷ lệ thưởng SMB
    /// </summary>
    [Display(Name = "📈 Tổng tỷ lệ thưởng SMB")]
    [Column("tong_ti_le_thuong_smb", TypeName = "decimal(18, 6)")]
    public decimal TotalSmbBonusRate { get; set; }

    /// <summary>
    /// Tổng giá trị SMB
    /// </summary>
    [Display(Name = "💵 Tổng giá trị SMB")]
    [Column("tong_gia_tri_smb", TypeName = "decimal(18, 6)")]
    public decimal TotalSmbValue { get; set; }

    /// <summary>
    /// Tổng tỷ lệ đợt 1
    /// </summary>
    [Display(Name = "📊 Tổng tỷ lệ đợt 1")]
    [Column("tong_ti_le_dot_1", TypeName = "decimal(18, 6)")]
    public decimal TotalPhase1Rate { get; set; }

    /// <summary>
    /// Tổng giá trị đợt 1
    /// </summary>
    [Display(Name = "💰 Tổng giá trị đợt 1")]
    [Column("tong_gia_tri_dot_1", TypeName = "decimal(18, 6)")]
    public decimal TotalPhase1Value { get; set; }

    /// <summary>
    /// Tổng thu hồi công nợ
    /// </summary>
    [Display(Name = "🔄 Tổng thu hồi công nợ")]
    [Column("tong_thu_hoi_cong_no", TypeName = "decimal(18, 6)")]
    public decimal TotalDebtRecovery { get; set; }

    /// <summary>
    /// Tổng nghiệm thu
    /// </summary>
    [Display(Name = "✅ Tổng nghiệm thu")]
    [Column("tong_nghiem_thu", TypeName = "decimal(18, 6)")]
    public decimal TotalAcceptance { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<SmbTeamBonus> SmbTeamBonuses { get; set; } = new List<SmbTeamBonus>();
}