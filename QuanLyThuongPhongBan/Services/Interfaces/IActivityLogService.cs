using QuanLyThuongPhongBan.Models.Entities;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task<IEnumerable<ActivityLog>> GetAllAsync();
        Task<ActivityLog?> GetByIdAsync(int id);
        Task<ActivityLog> CreateAsync(ActivityLog entity);
        Task<ActivityLog?> UpdateAsync(int id, ActivityLog entity);
        Task<bool> DeleteAsync(int id);
    }
}
