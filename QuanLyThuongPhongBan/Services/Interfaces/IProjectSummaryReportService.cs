using QuanLyThuongPhongBan.Models.App;
using QuanLyThuongPhongBan.Models.Entities;
using System.Collections.ObjectModel;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface IProjectSummaryReportService
    {
        Task<(List<ProjectBonus> projectBonuses,
            List<ProjectTeamBonus> projectTeamBonuses,
            List<ProjectBonusDetail> projectBonusDetails)> GetByIdAsync(int id);

        Task<ObservableCollection<Report>> GetAllReportAsync();

        Task<bool> CreateAsync(ObservableCollection<ProjectBonusDetail> projectBonusDetails);

        Task<bool> DeleteAsync(int id);

        //Task<bool> CreateSampleDataAsync();

    }
}
