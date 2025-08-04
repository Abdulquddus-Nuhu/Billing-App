using BillingApp.Application.Dtos.Responses;
using BillingApp.Domain.Enums;

namespace BillingApp.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<BaseResponse> Subscribe(string email, PlanType plan, bool autorenew);
        Task<BaseResponse> CancelSubscription(Guid subscriptionId);
        Task<BaseResponse> CancelActiveSubscription(string email);
        Task<BaseResponse> UpgradeOrDowngrade(string username, PlanType newPlan, bool autorenew);
        Task<SubscribeResponse> GetSubscription(string email);
        Task<IEnumerable<SubscribeResponse>> GetAllSubscriptions();
        Task<IEnumerable<SubscribeResponse>> GetUserSubscriptions(Guid userId);
        Task<List<RevenueSummary>> GetRevenueSummaryAsync();
    }
}
