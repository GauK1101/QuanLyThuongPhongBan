using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Data;
using QuanLyThuongPhongBan.Models.Entities;

namespace QuanLyThuongPhongBan.Utilities
{
    public static class ProjectBonusExcelPasteUtilities
    {
        // Method chính
        public static async Task<List<ProjectBonusDetail>> ProcessExcelPasteAsync(
                    List<List<string>> excelData,
                    List<int> selectedIds,
                    DataContext context)
        {
            var updatedEntities = new List<ProjectBonusDetail>();
            var newEntities = new List<ProjectBonusDetail>();

            // Nếu có selectedIds -> UPDATE mode
            if (selectedIds.Any())
            {
                // Lấy các entity được chọn từ database
                var entities = await context.ProjectBonusDetails
                    .Where(x => selectedIds.Contains(x.Id))
                    .ToListAsync();

                // Update từng entity với dữ liệu Excel (lặp theo số lượng row Excel)
                for (int i = 0; i < Math.Min(excelData.Count, entities.Count); i++)
                {
                    MapExcelDataToEntity(excelData[i], entities[i]);
                    updatedEntities.Add(entities[i]);
                }
            }
            // Nếu không có selectedIds -> CREATE mode
            else
            {
                // Tạo mới theo số lượng row trong Excel
                foreach (var rowData in excelData)
                {
                    var newEntity = CreateDefaultTemplate();
                    MapExcelDataToEntity(rowData, newEntity);
                    context.ProjectBonusDetails.Add(newEntity);
                    newEntities.Add(newEntity);
                }
            }

            await context.SaveChangesAsync();
            return updatedEntities.Concat(newEntities).ToList();
        }

        private static ProjectBonusDetail CreateDefaultTemplate()
        {
            return new ProjectBonusDetail
            {
                ProjectName = "Tên dự án mới",
                Investor = "Chủ đầu tư",
                ContractNumber = "Số hợp đồng",
                Date = DateTime.Now,
                Notes = "",

                // Revenue defaults
                ContractRevenue = 0m,
                SettlementRevenue = 0m,
                InvoicedRevenue = 0m,
                UninvoicedRevenue = 0m,

                // Payments defaults
                PaidAmount = 0m,
                UnpaidAmount = 0m,

                // Coefficients defaults
                HSDA = 0m,
                KTK = 0m,
                PO = 0m,
                TTDVKT = 0m,
                TVGP = 0m
            };
        }

        private static void MapExcelDataToEntity(List<string> excelData, ProjectBonusDetail entity)
        {
            if (excelData == null || !excelData.Any()) return;

            try
            {
                // BỎ QUA CỘT STT (index 0) - bắt đầu từ index 1
                if (excelData.Count >= 2) entity.Investor = excelData[1]; // Index 1 (bỏ STT)
                if (excelData.Count >= 3) entity.ContractNumber = excelData[2]; // Index 2
                if (excelData.Count >= 4) entity.ProjectName = excelData[3]; // Index 3

                // Xử lý ngày tháng
                if (excelData.Count >= 5 &&
                    DateTime.TryParseExact(excelData[4]?.Trim(), // Index 4
                                           new[] { "dd/MM/yy", "dd/MM/yyyy" },
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None,
                                           out DateTime date))
                    entity.Date = date;

                // Xử lý số tiền
                if (excelData.Count >= 6)
                    entity.ContractRevenue = ParseDecimalFromExcel(excelData[5]); // Index 5
                if (excelData.Count >= 7)
                    entity.SettlementRevenue = ParseDecimalFromExcel(excelData[6]); // Index 6
                if (excelData.Count >= 8)
                    entity.UninvoicedRevenue = ParseDecimalFromExcel(excelData[7]); // Index 7
                if (excelData.Count >= 9)
                    entity.InvoicedRevenue = ParseDecimalFromExcel(excelData[8]); // Index 8
                if (excelData.Count >= 10)
                    entity.PaidAmount = ParseDecimalFromExcel(excelData[9]); // Index 9
                if (excelData.Count >= 11)
                    entity.Notes = excelData[10]; // Index 10
                if (excelData.Count >= 12)
                    entity.TVGP = ParseDecimalFromExcel(excelData[11]); // Index 11
                if (excelData.Count >= 13)
                    entity.HSDA = ParseDecimalFromExcel(excelData[12]); // Index 12
                if (excelData.Count >= 14)
                    entity.PO = ParseDecimalFromExcel(excelData[13]); // Index 13
                if (excelData.Count >= 15)
                    entity.KTK = ParseDecimalFromExcel(excelData[14]); // Index 14
                if (excelData.Count >= 16)
                    entity.TTDVKT = ParseDecimalFromExcel(excelData[15]); // Index 15
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi map dữ liệu Excel: {ex.Message}");
            }
        }

        private static decimal ParseDecimalFromExcel(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0m;

            try
            {
                // Loại bỏ ký tự không phải số, dấu phẩy, dấu chấm
                var cleanValue = value.Replace(",", "").Replace(".", "").Trim();

                if (decimal.TryParse(cleanValue, out decimal result))
                    return result;

                return 0m;
            }
            catch
            {
                return 0m;
            }
        }

        // Helper để parse data từ clipboard
        public static List<List<string>> ParseExcelData(string clipboardData)
        {
            var result = new List<List<string>>();

            if (string.IsNullOrEmpty(clipboardData))
                return result;

            var rows = clipboardData.Split('\n');
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;

                var cells = row.Split('\t')
                              .Select(cell => cell.Trim())
                              .ToList();

                if (cells.Any(cell => !string.IsNullOrEmpty(cell)))
                    result.Add(cells);
            }

            return result;
        }
    }
}
