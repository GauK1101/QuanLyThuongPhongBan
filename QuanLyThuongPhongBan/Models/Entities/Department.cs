using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models.Entities
{
    /// <summary>
    /// Entity cho bảng Phòng ban (tb_phong_ban)
    /// Quản lý thông tin các phòng ban trong công ty
    /// </summary>
    [Table("tb_phong_ban")]
    [Display(Name = "🏢 Phòng ban")]
    public class Department
    {
        [Key]
        [Column("id")]
        [Display(Name = "🆔 ID")]
        public int Id { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        [Display(Name = "🏢 Tên phòng ban")]
        [Column("ten_phong_ban")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả
        /// </summary>
        [Display(Name = "📄 Mô tả")]
        [Column("mo_ta")]
        [StringLength(255)]
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
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
        public virtual ICollection<ProjectTeamBonus> ProjectTeamBonuses { get; set; } = new List<ProjectTeamBonus>();
        public virtual ICollection<SmbTeamBonus> SmbTeamBonuses { get; set; } = new List<SmbTeamBonus>();
    }
}
