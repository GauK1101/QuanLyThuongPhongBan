using QuanLyThuongPhongBan.Models.Entities;
using QuanLyThuongPhongBan.Models.Settings;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface IProjectBonusDetailService
    {
        Task<List<ProjectBonusDetail>> GetAllAsync();
        Task<(List<ProjectBonusDetail> Data, int TotalCount, int FilteredCount, int MaxPageCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        Task<ProjectBonusDetail?> GetByIdAsync(int id);
        Task<ProjectBonusDetail?> CreateAsync();
        Task<bool> UpdateAsync(int id, ProjectBonusDetail entity, ProjectBonusCalculateOptions? settings = null);
        Task<bool> DeleteAsync(int id);
        (bool isValid, string errorMessage) ValidateDateRange(DateTime? fromDate, DateTime? toDate);
        Task<List<ProjectBonusDetail>> PasteExcelDataAsync(List<List<string>> excelData, List<int> selectedIds);
    }
}
