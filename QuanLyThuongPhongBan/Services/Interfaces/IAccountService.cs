using QuanLyThuongPhongBan.Models.Entities;

namespace QuanLyThuongPhongBan.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task<Account> CreateAsync(Account entity);
        Task<Account?> UpdateAsync(int id, Account entity);
        Task<bool> DeleteAsync(int id);
    }
}
