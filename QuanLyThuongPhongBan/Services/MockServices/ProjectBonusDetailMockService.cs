using QuanLyThuongPhongBan.Helpers;
using QuanLyThuongPhongBan.Models.Entities;
using QuanLyThuongPhongBan.Services.Interfaces;

namespace QuanLyThuongPhongBan.Services.MockServices
{
    public class ProjectBonusDetailMockService
    {
        public async Task<List<ProjectBonusDetail>> GetAllAsync()
        {
            // ✅ MOCK DATA NẰM TRONG SERVICE
            await Task.Delay(100); // Giả lập async

            return new List<ProjectBonusDetail>
            {
                new ProjectBonusDetail 
                {
                    Id = 1,
                    ProjectBonusId = 1,
                    ProjectName = "Dự án xây dựng Chung cư Sunrise",
                    Investor = "Tập đoàn Sun Group",
                    ContractNumber = "HD-SUN-2024-001",
                    Date = new DateTime(2024, 1, 15),
                    Notes = "Dự án hoàn thành đúng tiến độ",
                    ContractRevenue = 5000000000m,        // 5 tỷ
                    SettlementRevenue = 5200000000m,      // 5.2 tỷ
                    InvoicedRevenue = 4800000000m,        // 4.8 tỷ
                    UninvoicedRevenue = 400000000m,       // 400 triệu
                    PaidAmount = 4500000000m,             // 4.5 tỷ
                    UnpaidAmount = 700000000m,            // 700 triệu
                    HSDA = 0.85m,
                    KTK = 0.90m,
                    PO = 0.75m,
                    TTDVKT = 0.80m,
                    TVGP = 0.95m
                },
                new ProjectBonusDetail
                {
                    Id = 2,
                    ProjectBonusId = 1,
                    ProjectName = "Dự án hệ thống ERP cho Ngân hàng ABC",
                    Investor = "Ngân hàng TMCP ABC",
                    ContractNumber = "HD-ERP-2024-002",
                    Date = new DateTime(2024, 2, 20),
                    Notes = "Đang triển khai giai đoạn 2",
                    ContractRevenue = 3000000000m,        // 3 tỷ
                    SettlementRevenue = 2800000000m,      // 2.8 tỷ
                    InvoicedRevenue = 2500000000m,        // 2.5 tỷ
                    UninvoicedRevenue = 300000000m,       // 300 triệu
                    PaidAmount = 2200000000m,             // 2.2 tỷ
                    UnpaidAmount = 600000000m,            // 600 triệu
                    HSDA = 0.78m,
                    KTK = 0.88m,
                    PO = 0.82m,
                    TTDVKT = 0.85m,
                    TVGP = 0.92m
                },
                new ProjectBonusDetail
                {
                    Id = 3,
                    ProjectBonusId = 1,
                    ProjectName = "Dự án cầu đường Bắc Nam",
                    Investor = "Bộ Giao thông Vận tải",
                    ContractNumber = "HD-GT-2024-003",
                    Date = new DateTime(2024, 3, 10),
                    Notes = "Dự án trọng điểm quốc gia",
                    ContractRevenue = 8000000000m,        // 8 tỷ
                    SettlementRevenue = 7500000000m,      // 7.5 tỷ
                    InvoicedRevenue = 7000000000m,        // 7 tỷ
                    UninvoicedRevenue = 500000000m,       // 500 triệu
                    PaidAmount = 6500000000m,             // 6.5 tỷ
                    UnpaidAmount = 1000000000m,           // 1 tỷ
                    HSDA = 0.92m,
                    KTK = 0.87m,
                    PO = 0.79m,
                    TTDVKT = 0.91m,
                    TVGP = 0.89m
                },
                new ProjectBonusDetail
                {
                    Id = 4,
                    ProjectBonusId = 2,
                    ProjectName = "Dự án nhà máy điện mặt trời",
                    Investor = "Tập đoàn Điện lực EVN",
                    ContractNumber = "HD-DL-2024-004",
                    Date = new DateTime(2024, 4, 5),
                    Notes = "Dự án năng lượng tái tạo",
                    ContractRevenue = 6000000000m,        // 6 tỷ
                    SettlementRevenue = 6200000000m,      // 6.2 tỷ
                    InvoicedRevenue = 5800000000m,        // 5.8 tỷ
                    UninvoicedRevenue = 400000000m,       // 400 triệu
                    PaidAmount = 5500000000m,             // 5.5 tỷ
                    UnpaidAmount = 700000000m,            // 700 triệu
                    HSDA = 0.88m,
                    KTK = 0.84m,
                    PO = 0.91m,
                    TTDVKT = 0.86m,
                    TVGP = 0.93m
                },
                new ProjectBonusDetail
                {
                    Id = 5,
                    ProjectBonusId = 2,
                    ProjectName = "Dự án nhà máy điện mặt trời",
                    Investor = "Tập đoàn Điện lực EVN",
                    ContractNumber = "HD-DL-2024-004",
                    Date = new DateTime(2024, 4, 5),
                    Notes = "Dự án năng lượng tái tạo",
                    ContractRevenue = 6000000000m,        // 6 tỷ
                    SettlementRevenue = 6200000000m,      // 6.2 tỷ
                    InvoicedRevenue = 5800000000m,        // 5.8 tỷ
                    UninvoicedRevenue = 400000000m,       // 400 triệu
                    PaidAmount = 5500000000m,             // 5.5 tỷ
                    UnpaidAmount = 700000000m,            // 700 triệu
                    HSDA = 0.88m,
                    KTK = 0.84m,
                    PO = 0.91m,
                    TTDVKT = 0.86m,
                    TVGP = 0.93m
                }
            };
        }

        public async Task<ProjectBonusDetail?> GetByIdAsync(int id)
        {
            await Task.Delay(100);
            return null;
        }

        public async Task<ProjectBonusDetail?> CreateAsync(ProjectBonusDetail? model = null)
        {
            await Task.Delay(100);
            return null;
        }

        public async Task<bool> UpdateAsync(int id, ProjectBonusDetail model)
        {
            await Task.Delay(100);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await Task.Delay(100);
            return true;
        }

        public async Task<bool> AddBonusDetailAsync(ProjectBonusDetail detail)
        {
            await Task.Delay(100);
            return true;
        }
    }
}
