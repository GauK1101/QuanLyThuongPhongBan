namespace QuanLyThuongPhongBan.Models.App.SettingsSmb;

public class CalculateOptionsSetting
{
    public bool CalculatePhase1Value { get; set; } = true;
    public bool CalculateTotalSmbValue { get; set; } = true;
    public bool CalculateDebtRecovery { get; set; } = true;
    public bool CalculateAcceptance { get; set; } = true;

    public bool AutoCalculateOnRateChange { get; set; } = true;

    // Để lưu file json đẹp
    public static CalculateOptionsSetting Default => new()
    {
        CalculatePhase1Value = true,
        CalculateTotalSmbValue = true,
        CalculateDebtRecovery = true,
        CalculateAcceptance = true,
        AutoCalculateOnRateChange = true
    };
}