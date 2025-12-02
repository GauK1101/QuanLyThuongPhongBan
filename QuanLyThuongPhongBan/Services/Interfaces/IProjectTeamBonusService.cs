using QuanLyThuongPhongBan.Models.Entities;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface IProjectTeamBonusService
    {
        Task<IEnumerable<ProjectTeamBonus>> GetAllAsync();
        Task<ProjectTeamBonus?> GetByIdAsync(int id);
        Task<ProjectTeamBonus> CreateAsync(ProjectTeamBonus entity);
        Task<ProjectTeamBonus?> UpdateAsync(int id, ProjectTeamBonus entity);
        Task<bool> DeleteAsync(int id);
    }
}
