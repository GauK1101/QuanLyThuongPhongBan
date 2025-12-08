
using Microsoft.Extensions.Options;
using QuanLyThuongPhongBan.Models.App.Settings;

namespace QuanLyThuongPhongBan.Utilities
{
    public class SmbRewardCalculatorUtilities
    {
        public static void CalculateSmbTeamBonuses(SmbBonus smbBonus, SmbCalculateOptions options)
        {
            if (smbBonus == null) return;

            // Tính toán cho từng chi tiết thưởng đại đoàn SMB
            foreach (var item in smbBonus.SmbTeamBonuses)
            {
                item.SmbRevenue = smbBonus.SmbRevenue;
                item.InvoiceRevenue = smbBonus.InvoiceRevenue;
                item.DebtRecoveryRevenue = smbBonus.DebtRecoveryRevenue;
                CalculateSingleTeamBonus(item, options);
            }

            // Tổng hợp các chỉ tiêu chung
            smbBonus.TotalPhase1Value = smbBonus.SmbTeamBonuses.Sum(d => d.Phase1Value);
            smbBonus.TotalPhase1Rate = smbBonus.SmbTeamBonuses.Sum(d => d.Phase1Rate);
            smbBonus.TotalSmbBonusValue = smbBonus.SmbTeamBonuses.Sum(d => d.TotalSmbValue);
            smbBonus.TotalAcceptance = smbBonus.SmbTeamBonuses.Sum(d => d.Acceptance);
            smbBonus.TotalDebtRecovery = smbBonus.SmbTeamBonuses.Sum(d => d.DebtRecovery);
            smbBonus.TotalSmbBonusRate = smbBonus.SmbTeamBonuses.Sum(d => d.TotalSmbRate);
        }

        public static void CalculateSingleTeamBonus(SmbTeamBonus item, SmbCalculateOptions options)
        {
            {

                if (item == null) return;

                if (options.CalculateTotalSmbValue)
                    item.TotalSmbValue = (item.SmbRevenue * item.TotalSmbRate) / 100;

                if (options.CalculatePhase1Value)
                    item.Phase1Value = (item.InvoiceRevenue * item.Phase1Rate) / 100;

                if (options.CalculateDebtRecovery)
                    item.DebtRecovery = (item.DebtRecoveryRevenue * (item.TotalSmbRate - item.Phase1Rate)) / 100;

                if (options.CalculateAcceptance)
                    item.Acceptance = item.Phase1Value + (item.TotalSmbValue - item.Phase1Value);
            }
        }
    }
}