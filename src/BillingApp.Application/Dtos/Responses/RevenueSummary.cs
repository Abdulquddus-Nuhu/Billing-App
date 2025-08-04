namespace BillingApp.Application.Dtos.Responses
{

    public class RevenueSummary
    {
        public string PlanType { get; set; } = string.Empty;
        public int ActiveCount { get; set; }
        public int ExpiredCount { get; set; }
        public decimal TotalRevenuePotential { get; set; }
    }
}
