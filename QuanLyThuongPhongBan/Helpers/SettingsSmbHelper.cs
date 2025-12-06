using QuanLyThuongPhongBan.Models.App.SettingsSmb;
using System.IO;
using System.Text.Json;

namespace QuanLyThuongPhongBan.Helpers;

public static class SettingsSmbHelper
{
    private static readonly string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "calculate-sbm-options.json");

    public static CalculateOptionsSetting LoadCalculateOptions()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<CalculateOptionsSetting>(json) ?? CalculateOptionsSetting.Default;
            }
        }
        catch { /* yên lặng là vàng */ }
        return CalculateOptionsSetting.Default;
    }

    public static void SaveCalculateOptions(CalculateOptionsSetting settings)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
        catch { }
    }
}