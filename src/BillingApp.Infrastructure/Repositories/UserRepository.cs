using BillingApp.Domain.Entities;
using BillingApp.Domain.Repositories;
using BillingApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BillingApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;
        private readonly ILogger<UserRepository> logger;
        private readonly UserManager<User> userManager;
        public UserRepository(AppDbContext context, ILogger<UserRepository> logger, UserManager<User> userManager)
        {
            this.context = context;
            this.logger = logger;
            this.userManager = userManager;
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                await context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while saving changes to the database.");
                return false;
            }
        }

        public Task DeleteAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(User user)
        {
            await SaveAsync();
            throw new NotImplementedException();
        }
    }
}
