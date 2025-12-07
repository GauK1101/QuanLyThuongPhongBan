using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuanLyThuongPhongBan.Data;
using QuanLyThuongPhongBan.Helpers;
using QuanLyThuongPhongBan.Models.App.Settings;
using QuanLyThuongPhongBan.Services.Interfaces;
using QuanLyThuongPhongBan.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace QuanLyThuongPhongBan.Services.Implementations
{
    class SmbRewardService : ISmbRewardService
    {
        private readonly IServiceProvider _serviceProvider;

        public SmbRewardService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private DataContext CreateNewContext()
        {
            return _serviceProvider.GetRequiredService<DataContext>();
        }

        private SmbBonus CreateDefaultTemplate()
        {
            return new SmbBonus
            {
                QuarterYear = DateTime.Now.Month + "-" + DateTime.Now.Year,
                SmbTeamBonuses = new List<SmbTeamBonus>
                {
                    new SmbTeamBonus
                    {
                        DepartmentId = 3,
                        Phase1Rate = 0.03m,
                        TotalSmbRate = 0.08m,
                        DebtRecoveryRate = 0.05m,
                    },
                    new SmbTeamBonus
                    {
                        DepartmentId = 4,
                        Phase1Rate = 0.10m,
                        TotalSmbRate = 0.25m,
                        DebtRecoveryRate = 0.15m,
                    },
                    new SmbTeamBonus
                    {
                        DepartmentId = 5,
                        Phase1Rate = 0.10m,
                        TotalSmbRate = 0.40m,
                        DebtRecoveryRate = 0.30m,
                    }
                }
            };
        }

        public async Task<List<SmbBonus>> GetAllAsync()
        {
            using var context = CreateNewContext();

            var data = await context.SmbBonuses.AsNoTracking().ToListAsync();
            return data;
        }

        public async Task<(List<SmbBonus> Data, int TotalCount, int FilteredCount, int MaxPageCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string searchKeyword = null)
        {
            using var context = CreateNewContext();

            var baseQuery = context.SmbBonuses
                .Include(sb => sb.SmbTeamBonuses)
                .ThenInclude(stb => stb.Department)
                .AsNoTracking();

            // Áp dụng filter theo từ khóa tìm kiếm
            var filteredQuery = baseQuery;

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                filteredQuery = filteredQuery.Where(sb =>
                    sb.QuarterYear.ToLower().Contains(searchKeyword.Trim())
                );
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

        public async Task<SmbBonus> GetByIdAsync(int id)
        {
            using var context = CreateNewContext();

            return id == 0
                ? new SmbBonus()
                : await context.SmbBonuses.FindAsync(id) ?? new SmbBonus();
        }

        public async Task<SmbBonus?> CreateAsync()
        {
            using var context = CreateNewContext();

            var entity = CreateDefaultTemplate();

            context.SmbBonuses.Add(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, SmbBonus model, SmbCalculateOptions? settings = null)
        {
            if (id == 0)
                return false;

            using var context = CreateNewContext();

            try
            {
                var entity = await context.SmbBonuses
                    .Include(sb => sb.SmbTeamBonuses)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sb => sb.Id == id);

                if (PropertyChangeHelper.HasAnyChanges(entity, model))
                {
                    //var changes = PropertyChangeHelper.GetChangesSummary(entity, model);
                    //MessageBox.Show(changes);

                    settings ??= SmbCalculateOptions.Default;

                    if (settings.AutoCalculateOnRateChange)
                        SmbRewardCalculatorUtilities.CalculateSmbTeamBonuses(model, settings);

                    context.Entry(model).State = EntityState.Modified;

                    foreach (var item in model.SmbTeamBonuses)
                    {
                        context.Entry(item).State = EntityState.Modified;
                    }

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

            context.SmbTeamBonuses.RemoveRange(context.SmbTeamBonuses.Where(x => x.SmbBonusId == id).ToList());
            context.SaveChanges();

            context.SmbBonuses.Remove(await GetByIdAsync(id));
            context.SaveChanges();
            return true;

        }
    }
}
