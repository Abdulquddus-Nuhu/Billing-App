using BillingApp.Domain.Entities;

namespace BillingApp.Domain.Repositories
{
    public interface ISubscriptionRepository
    {
        public Task<Subscription?> GetActiveSubscription(Guid userId);
        public Task<Subscription?> GetSubscription(Guid subscriptionId);
        public Task<bool> CreateSubscription(Subscription subscription);
        public Task<bool> UpdateSubscription(Subscription subscription);
        public IQueryable<Subscription> GetAllSubscriptions();
        public IQueryable<Subscription> GetUserSubscriptions(Guid userId);
    }
}
