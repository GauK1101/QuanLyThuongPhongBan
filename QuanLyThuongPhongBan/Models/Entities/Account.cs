﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuongPhongBan.Models.Entities
{
    /// <summary>
    /// Entity cho bảng Tài khoản (tb_tai_khoan)
    /// Quản lý thông tin đăng nhập và người dùng hệ thống
    /// </summary>
    [Table("tb_tai_khoan")]
    [Display(Name = "👤 Tài khoản")]
    public class Account
    {
        [Key]
        [Column("id")]
        [Display(Name = "🆔 ID")]
        public int Id { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        [Display(Name = "👤 Tên đăng nhập")]
        [Column("ten_dang_nhap")]
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Display(Name = "🔒 Mật khẩu")]
        [Column("mat_khau")]
        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Họ và tên
        /// </summary>
        [Display(Name = "👨‍💼 Họ và tên")]
        [Column("ho_ten")]
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// ID phòng ban
        /// </summary>
        [Display(Name = "🏢 Phòng ban")]
        [Column("id_phong_ban")]
        public int DepartmentId { get; set; }

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
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    }
}
