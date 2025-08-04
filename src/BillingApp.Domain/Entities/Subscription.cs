using BillingApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BillingApp.Domain.Entities
{
    public class Subscription
    {
        public Subscription()
        {
            Id = Guid.NewGuid();            
        }

        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public PlanType Plan { get; set; } 
        public DateTime ExpiryDate { get; set; }
        public bool AutoRenew { get; set; } = true;

        public User? User { get; set; }

        public decimal Price => Plan switch
        {
            PlanType.Basic => 1000,
            PlanType.Standard => 2000,
            PlanType.Premium => 3000,
            _ => 0
        };

        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

        public int BillingCycleInDays => Plan switch
        {
            PlanType.Basic => 30,
            PlanType.Standard => 90,
            PlanType.Premium => 30,
            _ => 30
        };

    }
}
