using BillingApp.Domain.Enums;
using BillingApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BillingApp.Infrastructure
{

    public class BillingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BillingBackgroundService> _logger;

        public BillingBackgroundService(IServiceProvider serviceProvider, ILogger<BillingBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Billing Background Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBillingCycle(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background service.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task ProcessBillingCycle(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var now = DateTime.UtcNow.Date;

            // 1. Send reminders for subscriptions ending in 3 days
            var reminderDate = now.AddDays(3);
            var subscriptionsToRemind = await context.Subscriptions
                .Include(s => s.User)
                .Where(s => s.Status == SubscriptionStatus.Active &&
                            s.ExpiryDate.Date == reminderDate &&
                            s.AutoRenew)
                .ToListAsync(ct);

            foreach (var sub in subscriptionsToRemind)
            {
                _logger.LogInformation("REMINDER: User {Email} - Your {Plan} subscription renews on {Date}",
                    sub.User.Email, nameof(sub.Plan), sub.ExpiryDate.ToString("yyyy-MM-dd"));
            }

            // 2. Auto-renew expired subscriptions
            var subscriptionsToRenew = await context.Subscriptions
                .Include(s => s.User)
                .Where(s => s.Status == SubscriptionStatus.Active &&
                            s.ExpiryDate.Date <= now &&
                            s.AutoRenew)
                .ToListAsync(ct);

            foreach (var sub in subscriptionsToRenew)
            {
                var user = sub.User;
                if (user.Balance >= sub.Price)
                {
                    user.Balance -= sub.Price;
                    sub.ExpiryDate = sub.ExpiryDate.AddDays(sub.BillingCycleInDays);
                    sub.Status = SubscriptionStatus.Active;

                    _logger.LogInformation("RENEWED: User {Email} - {Plan} renewed. New balance: {Balance}",
                        user.Email, nameof(sub.Plan), user.Balance);
                }
                else
                {
                    sub.Status = SubscriptionStatus.RenewalFailed;
                    _logger.LogWarning("RENEWAL FAILED: User {Email} - Insufficient balance for {Plan}",
                        user.Email, nameof(sub.Plan));
                }
            }

            await context.SaveChangesAsync(ct);
        }
    }
}
