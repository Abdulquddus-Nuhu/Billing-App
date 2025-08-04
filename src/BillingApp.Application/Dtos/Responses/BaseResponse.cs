namespace BillingApp.Application.Dtos.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
