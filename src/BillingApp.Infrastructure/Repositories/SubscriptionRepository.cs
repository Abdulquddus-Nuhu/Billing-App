using BillingApp.Domain.Entities;
using BillingApp.Domain.Enums;
using BillingApp.Domain.Repositories;
using BillingApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BillingApp.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext context;
        private readonly ILogger<SubscriptionRepository> logger;
        public SubscriptionRepository(AppDbContext context, ILogger<SubscriptionRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IQueryable<Subscription> GetUserSubscriptions(Guid userId)
        {
            return context.Subscriptions
                .Where(s => s.UserId == userId);
        }

        public async Task<Subscription?> GetActiveSubscription(Guid userId)
        {
            return await context.Subscriptions
                .FirstOrDefaultAsync(s => s.Status == SubscriptionStatus.Active && s.UserId == userId);
        }
     
        public async Task<Subscription?> GetSubscription(Guid subscriptionId)
        {
            return await context.Subscriptions
                .Where(s => s.Id == subscriptionId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateSubscription(Subscription subscription)
        {
            context.Subscriptions.Add(subscription);
            return await SaveAsync();
        }

        public async Task<bool> UpdateSubscription(Subscription subscription)
        {
            context.Subscriptions.Update(subscription);
            return await SaveAsync();
        }

        public IQueryable<Subscription> GetAllSubscriptions()
        {
            return context.Subscriptions;
        }

        public async Task<List<RevenueSummary>> GetRevenueSummaryAsync()
        {
            var subs = await context.Subscriptions.ToListAsync();

            return subs
                .GroupBy(s => s.Plan)
                .Select(g => new RevenueSummary
                {
                    PlanType = g.Key.ToString(),
                    ActiveCount = g.Count(s => s.Status == SubscriptionStatus.Active),
                    ExpiredCount = g.Count(s => s.Status == SubscriptionStatus.Expired || s.Status == SubscriptionStatus.RenewalFailed),
                    TotalRevenuePotential = g.Where(s => s.Status == SubscriptionStatus.Active).Sum(s => s.Price)
                }).ToList();
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



        public class RevenueSummary
        {
            public string PlanType { get; set; } = string.Empty;
            public int ActiveCount { get; set; }
            public int ExpiredCount { get; set; }
            public decimal TotalRevenuePotential { get; set; }
        }
    }
}
