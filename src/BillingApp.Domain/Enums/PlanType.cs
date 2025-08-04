namespace BillingApp.Domain.Enums
{
    public enum PlanType
    {
        /// <summary>
        /// 30 Days / Monthly
        /// </summary>
        Basic,
        /// <summary>
        /// 90 Days / Quaterly
        /// </summary>
        Standard,
        /// <summary>
        /// 30 Days / Monthly
        /// </summary>
        Premium,
    }

    public enum SubscriptionStatus
    {
        Active,
        Canceled,
        Expired,
        RenewalFailed
    }
}
