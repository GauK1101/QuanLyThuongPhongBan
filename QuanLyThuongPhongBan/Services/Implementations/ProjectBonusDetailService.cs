using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuanLyThuongPhongBan.Data;
using QuanLyThuongPhongBan.Helpers;
using QuanLyThuongPhongBan.Models.Entities;
using QuanLyThuongPhongBan.Services.Interfaces;
using QuanLyThuongPhongBan.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Windows;

namespace QuanLyThuongPhongBan.Services.Implementations
{
    class ProjectBonusDetailService : IProjectBonusDetailService
    {
        private readonly IServiceProvider _serviceProvider;

        public ProjectBonusDetailService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private DataContext CreateNewContext()
        {
            return _serviceProvider.GetRequiredService<DataContext>();
        }

        private ProjectBonusDetail CreateDefaultTemplate()
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

        public async Task<List<ProjectBonusDetail>> GetAllAsync()
        {
            using var context = CreateNewContext();

            var data = await context.ProjectBonusDetails.AsNoTracking().ToListAsync();
            return data;
        }

        public async Task<(List<ProjectBonusDetail> Data, int TotalCount, int FilteredCount, int MaxPageCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            using var context = CreateNewContext();

            var baseQuery = context.ProjectBonusDetails.AsNoTracking();

            // Áp dụng filter theo ngày
            var filteredQuery = baseQuery;
            if (fromDate.HasValue)
            {
                filteredQuery = filteredQuery.Where(x => x.Date >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                var toDateInclusive = toDate.Value.AddDays(1).AddSeconds(-1);
                filteredQuery = filteredQuery.Where(x => x.Date <= toDateInclusive);
            }

            // Lấy counts
            var totalCount = await baseQuery.CountAsync();
            var filteredCount = await filteredQuery.CountAsync();

            // Áp dụng phân trang
            var data = await filteredQuery
                .OrderBy(p => p.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var maxPageCount = (filteredCount + pageSize - 1) / pageSize;

            return (data, totalCount, filteredCount, maxPageCount);
        }

        public async Task<ProjectBonusDetail> GetByIdAsync(int id)
        {
            using var context = CreateNewContext();

            return id == 0
                ? new ProjectBonusDetail()
                : await context.ProjectBonusDetails.FindAsync(id) ?? new ProjectBonusDetail();
        }

        public async Task<ProjectBonusDetail?> CreateAsync()
        {
            using var context = CreateNewContext();

            // Tạo mới nếu model là null, ngược lại update
            var entity = CreateDefaultTemplate();

            context.ProjectBonusDetails.Add(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, ProjectBonusDetail model)
        {
            if (id == 0)
                return false;

            using var context = CreateNewContext();

            try
            {
                var entity = await GetByIdAsync(id);

                if (PropertyChangeHelper.HasAnyChanges(entity, model))
                {
                    //MessageBox.Show(PropertyChangeHelper.GetChangesSummary(entity, model).ToString());

                    if (HandyControl.Controls.MessageBox.Show("Bạn có muốn tự động sửa giá trị các không ?", "SỬA CHI TIẾT", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        ProjectBonusCalculatorUtilities.CalculateSingleDetail(model);

                    context.Entry(model).State = EntityState.Modified;
                    context.SaveChanges();

                    return true;
                }

                return false;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ValidationException("Đã có người chỉnh sửa nội dung này trước bạn.");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id == 0)
                return false;

            using var context = CreateNewContext();

            context.ProjectBonusDetails.Remove(await GetByIdAsync(id));
            context.SaveChanges();
            return true;

        }

        public (bool isValid, string errorMessage) ValidateDateRange(DateTime? fromDate, DateTime? toDate)
        {
            // Validation 1: Ngày bắt đầu > ngày kết thúc
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            {
                return (false, "❌ Ngày bắt đầu không thể lớn hơn ngày kết thúc");
            }

            // Validation 2: Ngày trong tương lai
            var now = DateTime.Now.Date;
            if (fromDate.HasValue && fromDate.Value.Date > now)
            {
                return (false, "❌ Ngày bắt đầu không thể lớn hơn ngày hiện tại");
            }

            if (toDate.HasValue && toDate.Value.Date > now)
            {
                return (false, "❌ Ngày kết thúc không thể lớn hơn ngày hiện tại");
            }

            // Validation 3: Khoảng thời gian quá lớn
            if (fromDate.HasValue && toDate.HasValue)
            {
                var timeSpan = toDate.Value - fromDate.Value;
                if (timeSpan.Days > 365) // 1 năm
                {
                    return (false, "⚠️ Khoảng thời gian filter không nên vượt quá 1 năm");
                }
            }

            // Validation 4: Ngày quá cũ (tuỳ chọn)
            if (fromDate.HasValue && fromDate.Value.Year < 2000)
            {
                return (false, "❌ Ngày bắt đầu không hợp lệ");
            }

            return (true, "✅ Dữ liệu hợp lệ");
        }

        public async Task<List<ProjectBonusDetail>> PasteExcelDataAsync(List<List<string>> excelData, List<int> selectedIds)
        {
            using var context = CreateNewContext();

            return await ProjectBonusExcelPasteUtilities.ProcessExcelPasteAsync(excelData, selectedIds, context);
        }
    }
}
