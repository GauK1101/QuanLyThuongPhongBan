using System.ComponentModel.DataAnnotations;

namespace QuanLyThuongPhongBan.Models.App
{
    [Display(Name = "📊 Báo cáo")]
    public class Report
    {
        /// <summary>
        /// ID báo cáo
        /// </summary>
        [Display(Name = "🆔 ID báo cáo")]
        public int Id { get; set; }

        /// <summary>
        /// Tên báo cáo
        /// </summary>
        [Display(Name = "📋 Tên báo cáo")]
        public string? NameReport { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [Display(Name = "📅 Ngày tạo")]
        public DateTime DateCrate { get; set; }
    }
}
