using QuanLyThuongPhongBan.Models.Settings;
using System.IO;
using System.Text.Json;

namespace QuanLyThuongPhongBan.Helpers.Setting
{
    public static class ProjectBonusCalculateOptionsHelper
    {
        private static readonly string ProjectBonusConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuanLyThuongPhongBan",
            "project-bonus-calculate-options.json");

        public static ProjectBonusCalculateOptions LoadCalculateOptions()
        {
            try
            {
                if (File.Exists(ProjectBonusConfigPath))
                {
                    var json = File.ReadAllText(ProjectBonusConfigPath);
                    return JsonSerializer.Deserialize<ProjectBonusCalculateOptions>(json) ?? ProjectBonusCalculateOptions.Default;
                }
            }
            catch { }
            return ProjectBonusCalculateOptions.Default;
        }

        public static void SaveCalculateOptions(ProjectBonusCalculateOptions options)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ProjectBonusConfigPath)!);
                var json = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ProjectBonusConfigPath, json);
            }
            catch { }
        }
    }
}
