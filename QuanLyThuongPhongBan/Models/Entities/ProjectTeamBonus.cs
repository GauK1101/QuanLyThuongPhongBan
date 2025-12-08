using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models.Entities
{
    /// <summary>
    /// Entity cho bảng Thưởng đại đoàn dự án (tb_thuong_dai_doan_du_an)
    /// Quản lý thưởng cho các đại đoàn tham gia dự án
    /// </summary>
    [Table("tb_thuong_dai_doan_du_an")]
    [Display(Name = "👥 Thưởng đại đoàn dự án")]
    public class ProjectTeamBonus
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

        /// <summary>
        /// ID thưởng dự án
        /// </summary>
        [Display(Name = "🏗️ ID thưởng dự án")]
        [Column("id_thuong_du_an")]
        public int ProjectBonusId { get; set; }

        /// <summary>
        /// Giá trị tổng gói
        /// </summary>
        [Display(Name = "💰 Giá trị tổng gói")]
        [Column("gia_tri_tong_goi", TypeName = "decimal(22, 6)")]
        public decimal TotalPackageValue { get; set; }

        /// <summary>
        /// Tỷ lệ tổng gói
        /// </summary>
        [Display(Name = "📊 Tỷ lệ tổng gói")]
        [Column("ti_le_tong_goi", TypeName = "decimal(22, 6)")]
        public decimal TotalPackageRate { get; set; }

        /// <summary>
        /// Giá trị điều chỉnh đợt 1
        /// </summary>
        [Display(Name = "💰 Giá trị điều chỉnh đợt 1")]
        [Column("gia_tri_dieu_chinh_dot_1", TypeName = "decimal(22, 6)")]
        public decimal Adjustment1Value { get; set; }

        /// <summary>
        /// Tỷ lệ điều chỉnh đợt 1
        /// </summary>
        [Display(Name = "📊 Tỷ lệ điều chỉnh đợt 1")]
        [DisplayFormat(DataFormatString = "{0:0.#####}%", ApplyFormatInEditMode = true)]
        [Column("ti_le_dieu_chinh_dot_1", TypeName = "decimal(22, 6)")]
        public decimal Adjustment1Rate { get; set; }

        /// <summary>
        /// Giá trị điều chỉnh đợt 2
        /// </summary>
        [Display(Name = "💰 Giá trị điều chỉnh đợt 2")]
        [Column("gia_tri_dieu_chinh_dot_2", TypeName = "decimal(22, 6)")]
        public decimal Adjustment2Value { get; set; }

        /// <summary>
        /// Tỷ lệ điều chỉnh đợt 2
        /// </summary>
        [Display(Name = "📊 Tỷ lệ điều chỉnh đợt 2")]
        [Column("ti_le_dieu_chinh_dot_2", TypeName = "decimal(22, 6)")]
        public decimal Adjustment2Rate { get; set; }

        /// <summary>
        /// Nghiệm thu
        /// </summary>
        [Display(Name = "✅ Nghiệm thu")]
        [Column("nghiem_thu", TypeName = "decimal(22, 6)")]
        public decimal Acceptance { get; set; }

        /// <summary>
        /// Thu hồi công nợ
        /// </summary>
        [Display(Name = "🔄 Thu hồi công nợ")]
        [Column("thu_hoi_cong_no", TypeName = "decimal(22, 6)")]
        public decimal DebtRecovery { get; set; }

        /// <summary>
        /// Doanh thu xuất hóa đơn
        /// </summary>
        [Display(Name = "🧾 Doanh thu xuất hóa đơn")]
        [Column("doanh_thu_xuat_hoa_don", TypeName = "decimal(22, 6)")]
        public decimal InvoiceRevenue { get; set; }

        /// <summary>
        /// Doanh thu hợp đồng
        /// </summary>
        [Display(Name = "📑 Doanh thu hợp đồng")]
        [Column("doanh_thu_hop_dong", TypeName = "decimal(22, 6)")]
        public decimal ContractRevenue { get; set; }

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
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; } = null!;

        [ForeignKey("ProjectBonusId")]
        public virtual ProjectBonus ProjectBonus { get; set; } = null!;
    }
}
