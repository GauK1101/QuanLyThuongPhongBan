namespace QuanLyThuongPhongBan.Models.App.Settings;

public class SmbCalculateOptions
{
    public bool CalculatePhase1Value { get; set; } = true;
    public bool CalculateTotalSmbValue { get; set; } = true;
    public bool CalculateDebtRecovery { get; set; } = true;
    public bool CalculateAcceptance { get; set; } = true;

    public bool AutoCalculateOnRateChange { get; set; } = true;

    // Để lưu file json đẹp
    public static SmbCalculateOptions Default => new()
    {
        CalculatePhase1Value = true,
        CalculateTotalSmbValue = true,
        CalculateDebtRecovery = true,
        CalculateAcceptance = true,
        AutoCalculateOnRateChange = true
    };
}
