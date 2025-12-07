using QuanLyThuongPhongBan.Models.App.Settings;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface ISmbRewardService
    {
        Task<List<SmbBonus>> GetAllAsync();
        Task<(List<SmbBonus> Data, int TotalCount, int FilteredCount, int MaxPageCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string searchKeyword = null);
        Task<SmbBonus?> GetByIdAsync(int id);
        Task<SmbBonus?> CreateAsync();
        Task<bool> UpdateAsync(int id, SmbBonus model, SmbCalculateOptions? settings = null);
        Task<bool> DeleteAsync(int id);
    }
}
