using QuanLyThuongPhongBan.Models.Entities;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface IProjectBonusService
    {
        Task<IEnumerable<ProjectBonus>> GetAllAsync();
        Task<ProjectBonus?> GetByIdAsync(int id);
        Task<ProjectBonus> CreateAsync(ProjectBonus entity);
        Task<ProjectBonus?> UpdateAsync(int id, ProjectBonus entity);
        Task<bool> DeleteAsync(int id);
    }
}
