using BillingApp.Domain.Entities;
using BillingApp.Domain.Enums;

namespace BillingApp.Application.Dtos.Responses
{
    public class SubscribeResponse : BaseResponse
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public string Plan { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool AutoRenew { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string Status { get; set; }

        public decimal Price { get; set; }
        public int BillingCycleInDays { get; set; }


    }
}
