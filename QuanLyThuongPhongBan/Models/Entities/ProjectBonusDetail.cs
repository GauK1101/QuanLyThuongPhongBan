using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models.Entities
{
    /// <summary>
    /// Entity cho bảng Thưởng dự án chi tiết (tb_thuong_du_an_chi_tiet)
    /// Quản lý chi tiết từng dự án và thông tin thanh toán
    /// </summary>
    [Table("tb_thuong_du_an_chi_tiet")]
    [Display(Name = "📋 Thưởng dự án chi tiết")]
    public class ProjectBonusDetail
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "🆔 ID")]
        public int Id { get; set; }

        /// <summary>
        /// ID thưởng dự án
        /// </summary>
        [Display(Name = "🏗️ ID thưởng dự án")]
        [Column("id_thuong_du_an")]
        public int? ProjectBonusId { get; set; }

        /// <summary>
        /// Tên dự án
        /// </summary>
        [Display(Name = "🏗️ Tên dự án")]
        [Column("du_an")]
        [StringLength(255)]
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// Chủ đầu tư
        /// </summary>
        [Display(Name = "👤 Chủ đầu tư")]
        [Column("chu_dau_tu")]
        [StringLength(255)]
        public string Investor { get; set; } = string.Empty;

        /// <summary>
        /// Số hợp đồng
        /// </summary>
        [Display(Name = "📑 Số hợp đồng")]
        [Column("hop_dong_so")]
        [StringLength(100)]
        public string ContractNumber { get; set; } = string.Empty;

        /// <summary>
        /// Ngày tháng
        /// </summary>
        [Display(Name = "📅 Ngày tháng")]
        [Column("ngay_thang")]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [Display(Name = "📝 Ghi chú")]
        [Column("ghi_chu")]
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Doanh thu hợp đồng
        /// </summary>
        [Display(Name = "💰 Doanh thu hợp đồng")]
        [Column("doanh_thu_hop_dong", TypeName = "decimal(22, 6)")]
        public decimal ContractRevenue { get; set; }

        /// <summary>
        /// Doanh thu quyết toán
        /// </summary>
        [Display(Name = "💰 Doanh thu quyết toán")]
        [Column("doanh_thu_quyet_toan", TypeName = "decimal(22, 6)")]
        public decimal SettlementRevenue { get; set; }

        /// <summary>
        /// Doanh thu đã xuất hóa đơn
        /// </summary>
        [Display(Name = "🧾 Doanh thu đã xuất hóa đơn")]
        [Column("doanh_thu_da_xuat_hoa_don", TypeName = "decimal(22, 6)")]
        public decimal InvoicedRevenue { get; set; }

        /// <summary>
        /// Doanh thu chưa xuất hóa đơn
        /// </summary>
        [Display(Name = "📄 Doanh thu chưa xuất hóa đơn")]
        [Column("doanh_thu_chua_xuat_hoa_don", TypeName = "decimal(22, 6)")]
        public decimal UninvoicedRevenue { get; set; }

        /// <summary>
        /// Đã thanh toán
        /// </summary>
        [Display(Name = "💳 Đã thanh toán")]
        [Column("da_thanh_toan", TypeName = "decimal(22, 6)")]
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// Chưa thanh toán
        /// </summary>
        [Display(Name = "⏳ Chưa thanh toán")]
        [Column("chua_thanh_toan", TypeName = "decimal(22, 6)")]
        public decimal UnpaidAmount { get; set; }

        /// <summary>
        /// Hệ số Dự án
        /// </summary>
        [Display(Name = "📊 Hệ số Dự án")]
        [Column("hsda", TypeName = "decimal(22, 6)")]
        public decimal HSDA { get; set; }

        /// <summary>
        /// Hệ số KTK
        /// </summary>
        [Display(Name = "📊 Hệ số KTK")]
        [Column("ktk", TypeName = "decimal(22, 6)")]
        public decimal KTK { get; set; }

        /// <summary>
        /// Hệ số PO
        /// </summary>
        [Display(Name = "📊 Hệ số PO")]
        [Column("po", TypeName = "decimal(22, 6)")]
        public decimal PO { get; set; }

        /// <summary>
        /// Hệ số TTDVKT
        /// </summary>
        [Display(Name = "📊 Hệ số TTDVKT")]
        [Column("ttdvkt", TypeName = "decimal(22, 6)")]
        public decimal TTDVKT { get; set; }

        /// <summary>
        /// Hệ số TVGP
        /// </summary>
        [Display(Name = "📊 Hệ số TVGP")]
        [Column("tvgp", TypeName = "decimal(22, 6)")]
        public decimal TVGP { get; set; }

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
        [ForeignKey("ProjectBonusId")]
        public virtual ProjectBonus? ProjectBonus { get; set; }
    }
}
