using BillingApp.Application.Dtos.Requests;
using BillingApp.Application.Dtos.Responses;
using BillingApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace RealtorHubAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [SwaggerTag("Authentication Endpoints")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]

        [SwaggerOperation(
          Summary = "Register A New User Endpoint")
         ]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(new BaseResponse() { Message = result.Message, Success = result.Success });
        }

        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [SwaggerOperation(
          Summary = "Login Endpoint",
          Description = "")
         ]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result.Success)
            {
                return Ok(result);
            }

            return Unauthorized(new BaseResponse() { Message = result.Message, Success = result.Success });
        }
    }
}
