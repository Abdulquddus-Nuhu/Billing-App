using BillingApp.Application.Dtos.Responses;
using BillingApp.Application.Interfaces;
using BillingApp.Domain.Entities;
using BillingApp.Domain.Enums;
using BillingApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BillingApp.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUserRepository userRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        public SubscriptionService(IUserRepository userRepository, ISubscriptionRepository subscriptionRepository)
        {
            this.userRepository = userRepository;
            this.subscriptionRepository = subscriptionRepository;
        }

        public async Task<BaseResponse> Subscribe(string email, PlanType plan, bool autorenew)
        {
            var response = new BaseResponse();

            var user = await userRepository.GetUserByEmail(email);
            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            var currentSub = await subscriptionRepository.GetActiveSubscription(user.Id);
            if (currentSub != null)
            {
                response.Success = false;
                response.Message = "User already has an active subscription. Please cancel the current subscription before subscribing to a new plan.";
                return response;
            }

            var newSub = new Subscription
            {
                UserId = user.Id,
                Plan = plan,
                AutoRenew = autorenew,
                Status = SubscriptionStatus.Active,
            };
            newSub.ExpiryDate = DateTime.Now.AddDays((int)newSub.BillingCycleInDays);


            if (await subscriptionRepository.CreateSubscription(newSub) != true)
            {
                response.Success = false;
                response.Message = "Failed to create subscription.";
                return response;
            }

            return new BaseResponse
            {
                Success = true,
                Message = "Subscription created successfully.",
            };

        }

        public async Task<BaseResponse> CancelSubscription(Guid subscriptionId)
        {
            var response = new BaseResponse();

            var currentSub = await subscriptionRepository.GetActiveSubscription(subscriptionId);
            if (currentSub is null)
            {
                response.Success = false;
                response.Message = "Subscription not found.";
                return response;
            }

            if (!(currentSub.Status is SubscriptionStatus.Active))
            {
                response.Success = false;
                response.Message = "Subscription is already inactive.";
                return response;
            }

            currentSub.Status = SubscriptionStatus.Canceled;

            if (await subscriptionRepository.UpdateSubscription(currentSub) != true)
            {
                response.Success = false;
                response.Message = "Failed to cancel subscription.";
                return response;
            }

            return new BaseResponse
            {
                Success = true,
                Message = "Subscription cancelled successfully.",
            };
        }

        public async Task<BaseResponse> CancelActiveSubscription(string email)
        {
            var response = new BaseResponse();

            var user = await userRepository.GetUserByEmail(email);
            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            var currentSub = await subscriptionRepository.GetActiveSubscription(user.Id);
            if (currentSub is null)
            {
                response.Success = false;
                response.Message = "Subscription not found.";
                return response;
            }

            if (!(currentSub.Status == SubscriptionStatus.Active))
            {
                response.Success = false;
                response.Message = "Subscription is already inactive.";
                return response;
            }

            currentSub.Status = SubscriptionStatus.Canceled;

            if (await subscriptionRepository.UpdateSubscription(currentSub) != true)
            {
                response.Success = false;
                response.Message = "Failed to cancel subscription.";
                return response;
            }

            return new BaseResponse
            {
                Success = true,
                Message = "Subscription cancelled successfully.",
            };
        }

        public async Task<BaseResponse> UpgradeOrDowngrade(string username, PlanType newPlan, bool autorenew)
        {
            var response = new BaseResponse();

            var user = await userRepository.GetUserByEmail(username);
            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            var currentSub = await subscriptionRepository.GetActiveSubscription(user.Id);
            if (currentSub is null)
            {
                response.Success = false;
                response.Message = "No active subscription found.";
                return response;
            }

            if (currentSub.Plan == newPlan)
            {
                response.Success = false;
                response.Message = "You are already subscribed to this plan.";
                return response;
            }

            // Ensure the current subscription is cancelled before proceeding
            var cancelPlanResult = await CancelActiveSubscription(username);
            if (!cancelPlanResult.Success)
            {
                response.Success = false;
                response.Message = cancelPlanResult.Message;
                return response;
            }
            else
            {
                var newSub = new Subscription
                {
                    UserId = user.Id,
                    Plan = newPlan,
                    Status = SubscriptionStatus.Active,
                    AutoRenew = autorenew 
                };
                newSub.ExpiryDate = DateTime.Now.AddDays((int)newSub.BillingCycleInDays);

                if (await subscriptionRepository.CreateSubscription(newSub) != true)
                {
                    response.Success = false;
                    response.Message = "Failed to Upgrade/Downgrade subscription.";
                    return response;
                }
            }

            response.Success = true;
            response.Message = "Subscription updated successfully.";
            return response;
        }

        public async Task<SubscribeResponse> GetSubscription(string email)
        {
            var response = new SubscribeResponse();

            var user = await userRepository.GetUserByEmail(email);
            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            var currentSub = await subscriptionRepository.GetActiveSubscription(user.Id);
            if (currentSub is null)
            {
                response.Success = false;
                response.Message = "No active subscription found.";
                return response;
            }

            response.Success = true;
            response.Message = "Subscription details retrieved successfully.";
            response.SubscriptionId = currentSub.Id;
            response.Plan = currentSub.Plan.ToString();
            response.Price = currentSub.Price;
            response.BillingCycleInDays = currentSub.BillingCycleInDays;
            response.ExpiryDate = currentSub.ExpiryDate;
            response.AutoRenew = currentSub.AutoRenew;
            response.Status = currentSub.Status.ToString();

            return response;
        }

        public async Task<IEnumerable<SubscribeResponse>> GetAllSubscriptions()
        {
            return await subscriptionRepository.GetAllSubscriptions()
                .Select(s => new SubscribeResponse
                {
                    SubscriptionId = s.Id,
                    UserId = s.UserId,
                    Plan = s.Plan.ToString(),
                    ExpiryDate = s.ExpiryDate,
                    AutoRenew = s.AutoRenew,
                    Status = s.Status.ToString(),
                    Price = s.Price,
                    BillingCycleInDays = s.BillingCycleInDays,
                    Success = true,
                    Message = "Subscription details retrieved successfully."
                }).ToListAsync();
        }

        public async Task<IEnumerable<SubscribeResponse>> GetUserSubscriptions(Guid userId)
        {
            return await subscriptionRepository.GetUserSubscriptions(userId)
                .Select(s => new SubscribeResponse
                {
                    SubscriptionId = s.Id,
                    UserId = s.UserId,
                    Plan = s.Plan.ToString(),
                    ExpiryDate = s.ExpiryDate,
                    AutoRenew = s.AutoRenew,
                    Status = s.Status.ToString(),
                    Price = s.Price,
                    BillingCycleInDays = s.BillingCycleInDays,
                    Success = true,
                    Message = "Subscription details retrieved successfully."
                }).ToListAsync();
        }


        public async Task<List<RevenueSummary>> GetRevenueSummaryAsync()
        {
            var subscriptions = await subscriptionRepository.GetAllSubscriptions().ToListAsync();

            return subscriptions
               .GroupBy(s => s.Plan)
               .Select(g => new RevenueSummary
               {
                   PlanType = g.Key.ToString(),
                   ActiveCount = g.Count(s => s.Status == SubscriptionStatus.Active),
                   ExpiredCount = g.Count(s => s.Status == SubscriptionStatus.Expired || s.Status == SubscriptionStatus.RenewalFailed),
                   TotalRevenuePotential = g.Where(s => s.Status == SubscriptionStatus.Active).Sum(s => s.Price)
               }).ToList();
        }
    }
}
