
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
                item.ContractRevenue = smbBonus.TotalSmbBonusValue;
                item.InvoiceRevenue = smbBonus.InvoiceOutput;
                //item. = smbBonus.TotalSmbBonusValue;
                CalculateSingleTeamBonus(item, options);
            }

            // Tổng hợp các chỉ tiêu chung
            smbBonus.TotalPhase1Value = smbBonus.SmbTeamBonuses.Sum(d => d.Phase1Value);
            smbBonus.TotalPhase1Rate = smbBonus.SmbTeamBonuses.Sum(d => d.Phase1Rate);
            smbBonus.TotalSmbValue = smbBonus.SmbTeamBonuses.Sum(d => d.TotalSmbValue);
            smbBonus.TotalAcceptance = smbBonus.SmbTeamBonuses.Sum(d => d.Acceptance);
            smbBonus.TotalDebtRecovery = smbBonus.SmbTeamBonuses.Sum(d => d.DebtRecovery);
            smbBonus.TotalSmbBonusRate = smbBonus.SmbTeamBonuses.Sum(d => d.TotalSmbRate);

            // Tính tổng xuất hóa đơn từ các chi tiết
            //smbBonus.TotalSmbBonusValue = smbBonus.SmbTeamBonuses.Sum(d => d.TotalSmbValue);
            //smbBonus.InvoiceOutput = smbBonus.SmbTeamBonuses.Sum(d => d.InvoiceRevenue);
        }

        public static void CalculateSingleTeamBonus(SmbTeamBonus item, SmbCalculateOptions options)
        {
            {
                //if (item == null) return;

                //// Tính giá trị tổng SMB = (Doanh thu hợp đồng * Tỷ lệ tổng SMB) / 100
                //if (isTotalSmbValue)
                //    item.TotalSmbValue = (item.ContractRevenue * item.TotalSmbRate) / 100;

                //// Tính giá trị đợt 1 = (Doanh thu xuất hóa đơn * Tỷ lệ đợt 1) / 100
                //if (isPhase1Value)
                //    item.Phase1Value = (item.InvoiceRevenue * item.Phase1Rate) / 100;

                //// Tính thu hồi công nợ = Tổng SMB - Đợt 1
                ////item.DebtRecovery = item.TotalSmbValue - item.Phase1Value;

                //// Tính tỷ lệ thu hồi công nợ = Tỷ lệ tổng SMB - Tỷ lệ đợt 1
                //if (isDebtRecoveryRate)
                //    item.DebtRecoveryRate = item.TotalSmbRate - item.Phase1Rate;

                //// Tính nghiệm thu = Đợt 1 + Thu hồi công nợ
                //if (isAcceptance)
                //    item.Acceptance = item.Phase1Value + item.DebtRecovery;

                if (item == null) return;

                if (options.CalculateTotalSmbValue)
                    item.TotalSmbValue = (item.ContractRevenue * item.TotalSmbRate) / 100;

                if (options.CalculatePhase1Value)
                    item.Phase1Value = (item.InvoiceRevenue * item.Phase1Rate) / 100;

                if (options.CalculateDebtRecovery)
                    item.DebtRecoveryRate = item.TotalSmbRate - item.Phase1Rate;

                if (options.CalculateAcceptance)
                    item.Acceptance = item.Phase1Value + (item.TotalSmbValue - item.Phase1Value);
            }
        }
    }
}