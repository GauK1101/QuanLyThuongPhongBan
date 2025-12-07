namespace QuanLyThuongPhongBan.Models.Settings;

public class ProjectBonusCalculateOptions
{
    public bool AutoCalculateOnRateChange { get; set; } = true;

    //public bool CalculateTotalPackageValue { get; set; } = true;
    //public bool CalculateAdjustment1Value { get; set; } = true;
    //public bool CalculateAdjustment2Value { get; set; } = true;
    //public bool CalculateDebtRecovery { get; set; } = true;
    //public bool CalculateAcceptance { get; set; } = true;

    public bool CalculateDetailTVGP { get; set; } = true;
    public bool CalculateDetailHSDA { get; set; } = true;
    public bool CalculateDetailPO { get; set; } = true;
    public bool CalculateDetailKTK { get; set; } = true;
    public bool CalculateDetailTTDVKT { get; set; } = true;
    public bool CalculateUnpaidAmount { get; set; } = true;
    public bool CalculateUninvoicedRevenue { get; set; } = true;

    public static ProjectBonusCalculateOptions Default => new()
    {
        AutoCalculateOnRateChange = true,
        CalculateDetailTVGP = true,
        CalculateDetailHSDA = true,
        CalculateDetailPO = true,
        CalculateDetailKTK = true,
        CalculateDetailTTDVKT = true,
        CalculateUnpaidAmount = true,
        CalculateUninvoicedRevenue = true
    };
}