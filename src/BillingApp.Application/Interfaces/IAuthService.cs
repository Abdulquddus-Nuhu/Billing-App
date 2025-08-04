using BillingApp.Application.Dtos.Requests;
using BillingApp.Application.Dtos.Responses;

namespace BillingApp.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResponse> RegisterAsync(RegisterUserRequest request);
        public Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
