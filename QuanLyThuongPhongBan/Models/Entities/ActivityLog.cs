using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models.Entities
{
    /// <summary>
    /// Entity cho bảng Nhật ký hoạt động (tb_nhat_ky)
    /// Ghi lại tất cả các hành động của người dùng trong hệ thống
    /// </summary>
    [Table("tb_nhat_ky")]
    [Display(Name = "📝 Nhật ký hoạt động")]
    public class ActivityLog
    {
        [Key]
        [Column("id")]
        [Display(Name = "🆔 ID")]
        public int Id { get; set; }

        /// <summary>
        /// ID tài khoản
        /// </summary>
        [Display(Name = "👤 ID tài khoản")]
        [Column("id_tai_khoan")]
        [Required]
        [StringLength(50)]
        public string AccountId { get; set; } = string.Empty;

        /// <summary>
        /// Hành động
        /// </summary>
        [Display(Name = "⚡ Hành động")]
        [Column("hanh_dong")]
        [Required]
        [StringLength(200)]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả
        /// </summary>
        [Display(Name = "📄 Mô tả")]
        [Column("mo_ta")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

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
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;
    }
}
