using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuanLyThuongPhongBan.Data;
using QuanLyThuongPhongBan.Models.App;
using QuanLyThuongPhongBan.Models.Entities;
using QuanLyThuongPhongBan.Services.Interfaces;
using QuanLyThuongPhongBan.Utilities;
using System.Collections.ObjectModel;

namespace QuanLyThuongPhongBan.Services.Implementations
{
    class ProjectSummaryReportService : IProjectSummaryReportService
    {
        private readonly IServiceProvider _serviceProvider;

        public ProjectSummaryReportService(IServiceProvider serviceProvider)
        {

            _serviceProvider = serviceProvider;
        }

        private DataContext CreateNewContext()
        {
            return _serviceProvider.GetRequiredService<DataContext>();
        }


        public async Task<(List<ProjectBonus> projectBonuses,
                      List<ProjectTeamBonus> projectTeamBonuses,
                      List<ProjectBonusDetail> projectBonusDetails)> GetByIdAsync(int id)
        {
            using var context = CreateNewContext();

            var projectBonusesTask = context.ProjectBonuses
                .AsNoTracking()
                .Where(x => x.Id == id)
                .ToListAsync();

            var projectTeamBonusesTask = context.ProjectTeamBonuses
                .AsNoTracking()
                .Where(x => x.ProjectBonusId == id)
                .Include(x => x.Department)
                .Include(x => x.ProjectBonus)
                .ToListAsync();

            var projectBonusDetailsTask = context.ProjectBonusDetails
                .AsNoTracking()
                .Where(x => x.ProjectBonusId == id)
                .Include(x => x.ProjectBonus)
                .ToListAsync();

            // Chạy cả 3 task cùng lúc
            await Task.WhenAll(projectBonusesTask, projectTeamBonusesTask, projectBonusDetailsTask);

            return (projectBonusesTask.Result, projectTeamBonusesTask.Result, projectBonusDetailsTask.Result);
        }

        public async Task<ObservableCollection<Report>> GetAllReportAsync()
        {
            // ✅ Tách riêng query và processing
            List<ProjectBonus> data;

            using (var context = CreateNewContext())
            {
                data = await context.ProjectBonuses.AsNoTracking().ToListAsync();
            } // 🔥 Context disposed ở đây - SAU KHI query hoàn thành

            // Xử lý data sau khi context đã disposed
            var reports = data
                .GroupBy(pb => pb.Year)
                .SelectMany(group => group.Select((pb, index) => new Report
                {
                    Id = pb.Id,
                    NameReport = $"Thưởng dự án năm {group.Key} - lần {index + 1}",
                    DateCrate = pb.CreatedAt,
                }))
                .OrderByDescending(pb => pb.Id)
                .ToList();

            return new ObservableCollection<Report>(reports);
        }

        public async Task<bool> CreateAsync(ObservableCollection<ProjectBonusDetail> projectBonusDetails)
        {
            using var context = CreateNewContext();
            // Tạo dữ liệu Departments nếu chưa có
            if (!await context.Departments.AnyAsync())
            {
                var departments = new List<Department>
                             {
                                 new Department { Name = "Tư vấn giải pháp", Description = "" },
                                 new Department { Name = "Hồ sơ dự án", Description = "" },
                                 new Department { Name = "Nhập hàng", Description = "" },
                                 new Department { Name = "Kế toán - kho", Description = "" },
                                 new Department { Name = "TT kỹ thuật & DV", Description = "" }
                             };
                await context.Departments.AddRangeAsync(departments);
                await context.SaveChangesAsync();
            }

            var projectBonuses = new ProjectBonus
            {
                Year = DateTime.Now.Year.ToString(),
            };

            await context.ProjectBonuses.AddRangeAsync(projectBonuses);
            await context.SaveChangesAsync();

            // Lấy departments đã có
            var existingDepartments = await context.Departments.AsNoTracking().ToListAsync();

            // Tạo dữ liệu ProjectTeamBonuses (tb_thuong_dai_doan_du_an)
            var projectTeamBonuses = new List<ProjectTeamBonus>();

            decimal[] tongTiLeThuongDuAn = { 0.5m, 0.25m, 0.07m, 0.25m, 0.5m };
            decimal[] tiLeDieuChinhDot1 = { 0.025m, 0.015m, 0, 0, 0 };
            decimal[] tiLeDieuChinhDot2 = { 0.125m, 0.025m, 0.025m, 0.05m, 0.1m };

            // Khởi tạo chi tiết thưởng đại đoàn cho từng phòng ban
            for (int i = 1; i <= tongTiLeThuongDuAn.Length; i++)
            {
                projectTeamBonuses.Add(
                    new ProjectTeamBonus
                    {
                        DepartmentId = i,
                        ProjectBonusId = projectBonuses.Id,
                        TotalPackageRate = tongTiLeThuongDuAn[i - 1],
                        Adjustment1Rate = tiLeDieuChinhDot1[i - 1],
                        Adjustment2Rate = tiLeDieuChinhDot2[i - 1]
                    }
                );
            }

            await context.ProjectTeamBonuses.AddRangeAsync(projectTeamBonuses);
            await context.SaveChangesAsync();

            // Tạo dữ liệu ProjectBonusDetails (tb_thuong_du_an_chi_tiet)
            projectBonusDetails.ToList().ForEach(item => item.ProjectBonusId = projectBonuses.Id);

            context.ChangeTracker.Clear();
            context.ProjectBonusDetails.UpdateRange(projectBonusDetails);
            await context.SaveChangesAsync();

            var entity = await context.ProjectBonuses
                .Include(x => x.ProjectTeamBonuses)
                .Include(x => x.ProjectBonusDetails)
                .FirstOrDefaultAsync(x => x.Id == projectBonuses.Id);
            ProjectBonusCalculatorUtilities.CalculateProjectTeamBonuses(entity);
            await context.SaveChangesAsync();

            // Detach sau khi save
            context.ChangeTracker.Clear();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var context = CreateNewContext();
            var projectBonusDetails = await context.ProjectBonusDetails
                .Where(x => x.ProjectBonusId == id)
                .ToListAsync();

            projectBonusDetails.ForEach(item => item.ProjectBonusId = null);

            context.ProjectBonusDetails.UpdateRange(projectBonusDetails);
            await context.SaveChangesAsync();

            context.ProjectTeamBonuses.RemoveRange(context.ProjectTeamBonuses.Where(x => x.ProjectBonusId == id));
            context.ProjectBonuses.RemoveRange(context.ProjectBonuses.Where(x => x.Id == id));

            await context.SaveChangesAsync();

            return true;
        }
    }
}
