using BillingApp.Application.Dtos.Requests;
using BillingApp.Application.Dtos.Responses;
using BillingApp.Application.Interfaces;
using BillingApp.Domain.Entities;
using BillingApp.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BillingApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AuthService> logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterUserRequest request)
        {
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Role = RoleType.User,
            };

            _logger.LogInformation("Creating a new user with {Email}", request.Email);

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with {Email} created successfully", request.Email);
                _logger.LogInformation("Sending welcome email to {Email}.", request.Email);
                await _userManager.AddToRoleAsync(user, "User");

                var token = await GenerateJwtToken(user);
                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email!,
                    FullName = $"{user.FirstName} {user.LastName}"
                };
            }

            return new AuthResponse
            {
                Success = false,
                Message = string.Join(',', result.Errors.Select(a => a.Description))
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Email);
            if (user == null)
                return new AuthResponse { Success = false, Message = "User account not found!" };

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return new AuthResponse { Success = false, Message = "Invalid credentials!" };

            var token = await GenerateJwtToken(user);
            return new AuthResponse
            {
                Success = true,
                Token = token,
                UserId = user.Id,
                Email = user.Email!,
                FullName = $"{user.FirstName} {user.LastName}"
            };
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT_SECRET_KEY"]
                ?? throw new InvalidOperationException("JWT_SECRET_KEY is not set"));

            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("fullName", $"{user.FirstName} {user.LastName}"),
                new Claim("role", $"{nameof(user.Role)}")
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
