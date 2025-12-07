using QuanLyThuongPhongBan.Models.App.Settings;
using System.IO;
using System.Text.Json;

namespace QuanLyThuongPhongBan.Helpers.Setting;

public static class SmbCalculateOptionsHelper
{
    private static readonly string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "sbm-calculate-options.json");

    public static SmbCalculateOptions LoadCalculateOptions()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<SmbCalculateOptions>(json) ?? SmbCalculateOptions.Default;
            }
        }
        catch { /* yên lặng là vàng */ }
        return SmbCalculateOptions.Default;
    }

    public static void SaveCalculateOptions(SmbCalculateOptions settings)
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