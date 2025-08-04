using BillingApp.Domain.Entities;
using BillingApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BillingApp.Infrastructure
{
    public class SeedDb : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedDb(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedDb>>();
            try
            {
                logger.LogInformation("Applying BillingApp_Db Migration!");
                //await context.Database.EnsureCreatedAsync();
                await context.Database.MigrateAsync(cancellationToken: cancellationToken);
                logger.LogInformation("BillingApp_Db Migration Successful!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to apply BillingApp_Db Migration!");
            }
            var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<Role>>();
            try
            {
                logger.LogInformation("Seeding BillingApp_Db Data!");
                await SeedIdentity.SeedAsync(userManager, roleManager, config);
                logger.LogInformation("Seeding BillingApp_Db Successful!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to execute BillingApp_Db Data Seeding!");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
