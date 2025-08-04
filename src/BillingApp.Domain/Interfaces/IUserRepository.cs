using BillingApp.Domain.Entities;

namespace BillingApp.Domain.Repositories
{
    public interface IUserRepository
    {   
        Task<User?> GetUserByEmail(string email);
        Task<bool> SaveAsync();
        Task<IEnumerable<User>> GetAll();
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid userId);
    }
}
