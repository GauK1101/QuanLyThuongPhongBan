using QuanLyThuongPhongBan.Models.App.Settings;
using QuanLyThuongPhongBan.Models.Entities;
using QuanLyThuongPhongBan.Models.Settings;

namespace QuanLyThuongPhongBan.Utilities
{
    public class ProjectBonusCalculatorUtilities
    {
        public static void CalculateProjectTeamBonuses(ProjectBonus projectBonus)
        {
            if (projectBonus == null) return;

            // --- Cập nhật tổng hợp doanh thu ---
            projectBonus.ContractValue = projectBonus.ProjectBonusDetails.Sum(d => d.ContractRevenue);
            projectBonus.Settlement = projectBonus.ProjectBonusDetails.Sum(d => d.SettlementRevenue);

            // --- Phân bổ doanh thu theo phòng ban ---
            var filterByDepartment = new Dictionary<int, Func<ProjectBonusDetail, bool>>
            {
                { 1, d => d.TVGP > 0 },  // TVGP
                { 2, d => d.HSDA > 0 },  // HSDA
                { 3, d => d.PO > 0 },    // PO
                { 4, d => d.KTK > 0 },   // KTK
                { 5, d => d.TTDVKT > 0 } // TTDVKT
            };

            foreach (var item in projectBonus.ProjectTeamBonuses)
            {
                if (filterByDepartment.TryGetValue(item.DepartmentId, out var condition))
                {
                    item.ContractRevenue = projectBonus.ProjectBonusDetails.Where(condition).Sum(d => d.ContractRevenue);
                    item.InvoiceRevenue = projectBonus.ProjectBonusDetails.Where(condition).Sum(d => d.SettlementRevenue);

                        item.TotalPackageValue = (item.InvoiceRevenue * item.TotalPackageRate) / 100;
                        item.Adjustment1Value = (item.ContractRevenue * item.Adjustment1Rate) / 100;
                        item.Adjustment2Value = (item.ContractRevenue * item.Adjustment2Rate) / 100;

                        item.DebtRecovery = item.TotalPackageValue - item.Adjustment1Value - item.Adjustment2Value;
                        item.Acceptance = item.Adjustment1Value + item.Adjustment2Value + item.DebtRecovery;
                }
                else
                {
                    item.ContractRevenue = 0m;
                    item.InvoiceRevenue = 0m;
            }
            }

            // --- Tổng hợp lại các chỉ tiêu chung ---
            projectBonus.TotalProjectBonusRate = projectBonus.ProjectTeamBonuses.Sum(d => d.TotalPackageRate);
            projectBonus.TotalProjectBonusValue = projectBonus.ProjectTeamBonuses.Sum(d => d.TotalPackageValue);
            projectBonus.TotalAdjustment1Rate = projectBonus.ProjectTeamBonuses.Sum(d => d.Adjustment1Rate);
            projectBonus.TotalAdjustment1Value = projectBonus.ProjectTeamBonuses.Sum(d => d.Adjustment1Value);
            projectBonus.TotalAdjustment2Rate = projectBonus.ProjectTeamBonuses.Sum(d => d.Adjustment2Rate);
            projectBonus.TotalAdjustment2Value = projectBonus.ProjectTeamBonuses.Sum(d => d.Adjustment2Value);
            projectBonus.TotalDebtRecovery = projectBonus.ProjectTeamBonuses.Sum(d => d.DebtRecovery);
            projectBonus.TotalAcceptance = projectBonus.ProjectTeamBonuses.Sum(d => d.Acceptance);
        }

        //public static void CalculateDetails(ICollection<ProjectBonusDetail> details)
        //{
        //    if (details == null || !details.Any()) return;

        //    foreach (var item in details)
        //    {
        //        CalculateSingleDetail(item);
        //    }
        //}

        public static void CalculateSingleDetail(ProjectBonusDetail item, ProjectBonusCalculateOptions options)
        {
            if (item == null) return;

            // Tính phần thưởng theo từng phòng ban (1-5)
            if (options.CalculateDetailTVGP) item.TVGP = Math.Round(item.SettlementRevenue * 2.5m / 100m, 0);
            if (options.CalculateDetailHSDA) item.HSDA = Math.Round(item.SettlementRevenue * 5.0m / 100m, 0);
            if (options.CalculateDetailPO) item.PO = Math.Round(item.SettlementRevenue * 3.0m / 100m, 0);
            if (options.CalculateDetailKTK) item.KTK = Math.Round(item.SettlementRevenue * 4.0m / 100m, 0);
            if (options.CalculateDetailTTDVKT) item.TTDVKT = Math.Round(item.SettlementRevenue * 2.0m / 100m, 0);

            // Tính DoanhThuChuaXuatHoaDon & ChuaThanhToan
            if (options.CalculateUninvoicedRevenue) item.UninvoicedRevenue = item.SettlementRevenue - item.InvoicedRevenue;
            if (options.CalculateUnpaidAmount) item.UnpaidAmount = item.SettlementRevenue - item.PaidAmount;
        }
    }
}
