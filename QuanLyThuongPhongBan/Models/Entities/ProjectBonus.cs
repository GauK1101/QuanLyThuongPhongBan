using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models.Entities
{
    /// <summary>
    /// Entity cho bảng Thưởng dự án (tb_thuong_du_an)
    /// Quản lý thông tin thưởng tổng hợp cho các dự án
    /// </summary>
    [Table("tb_thuong_du_an")]
    [Display(Name = "🏗️ Thưởng dự án")]
    public class ProjectBonus
    {
        [Key]
        [Column("id")]
        [Display(Name = "🆔 ID")]
        public int Id { get; set; }

        /// <summary>
        /// Năm thưởng
        /// </summary>
        [Display(Name = "📅 Năm thưởng")]
        [Column("nam_thuong")]
        [StringLength(4)]
        public string Year { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị hợp đồng
        /// </summary>
        [Display(Name = "💰 Giá trị hợp đồng")]
        [Column("gia_tri_hop_dong", TypeName = "decimal(22, 6)")]
        public decimal ContractValue { get; set; }

        /// <summary>
        /// Quyết toán
        /// </summary>
        [Display(Name = "📋 Quyết toán")]
        [Column("quyet_toan", TypeName = "decimal(22, 6)")]
        public decimal Settlement { get; set; }

        /// <summary>
        /// Tổng giá trị điều chỉnh đợt 1
        /// </summary>
        [Display(Name = "💰 Tổng giá trị điều chỉnh đợt 1")]
        [Column("tong_gia_tri_dieu_chinh_dot_1", TypeName = "decimal(22, 6)")]
        public decimal TotalAdjustment1Value { get; set; }

        /// <summary>
        /// Tổng tỷ lệ điều chỉnh đợt 1
        /// </summary>
        [Display(Name = "📊 Tổng tỷ lệ điều chỉnh đợt 1")]
        [Column("tong_ti_le_dieu_chinh_dot_1", TypeName = "decimal(22, 6)")]
        public decimal TotalAdjustment1Rate { get; set; }

        /// <summary>
        /// Tổng giá trị điều chỉnh đợt 2
        /// </summary>
        [Display(Name = "💰 Tổng giá trị điều chỉnh đợt 2")]
        [Column("tong_gia_tri_dieu_chinh_dot_2", TypeName = "decimal(22, 6)")]
        public decimal TotalAdjustment2Value { get; set; }

        /// <summary>
        /// Tổng tỷ lệ điều chỉnh đợt 2
        /// </summary>
        [Display(Name = "📊 Tổng tỷ lệ điều chỉnh đợt 2")]
        [Column("tong_ti_le_dieu_chinh_dot_2", TypeName = "decimal(22, 6)")]
        public decimal TotalAdjustment2Rate { get; set; }

        /// <summary>
        /// Tổng nghiệm thu
        /// </summary>
        [Display(Name = "✅ Tổng nghiệm thu")]
        [Column("tong_nghiem_thu", TypeName = "decimal(22, 6)")]
        public decimal TotalAcceptance { get; set; }

        /// <summary>
        /// Tổng thu hồi công nợ
        /// </summary>
        [Display(Name = "🔄 Tổng thu hồi công nợ")]
        [Column("tong_thu_hoi_cong_no", TypeName = "decimal(22, 6)")]
        public decimal TotalDebtRecovery { get; set; }

        /// <summary>
        /// Tổng giá trị thưởng dự án
        /// </summary>
        [Display(Name = "🎁 Tổng giá trị thưởng dự án")]
        [Column("tong_gia_tri_thuong_du_an", TypeName = "decimal(22, 6)")]
        public decimal TotalProjectBonusValue { get; set; }

        /// <summary>
        /// Tổng tỷ lệ thưởng dự án
        /// </summary>
        [Display(Name = "📈 Tổng tỷ lệ thưởng dự án")]
        [Column("tong_ti_le_thuong_du_an", TypeName = "decimal(22, 6)")]
        public decimal TotalProjectBonusRate { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        [Display(Name = "⏰ Thời gian tạo")]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Thời gian cập nhật
        /// </summary>
        [Display(Name = "🔄 Thời gian cập nhật")]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<ProjectTeamBonus> ProjectTeamBonuses { get; set; } = new List<ProjectTeamBonus>();
        public virtual ICollection<ProjectBonusDetail> ProjectBonusDetails { get; set; } = new List<ProjectBonusDetail>();
    }
}
