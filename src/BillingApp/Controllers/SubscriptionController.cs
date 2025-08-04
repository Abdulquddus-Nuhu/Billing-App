using BillingApp.Application.Dtos.Responses;
using BillingApp.Application.Interfaces;
using BillingApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;
using System.Security.Claims;

namespace BillingApp.Controllers
{
    [Route("api/subscription")]
    [Authorize]
    [ApiController]
    [SwaggerTag("Subscription Endpoints")]

    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(IEnumerable<SubscribeResponse>), StatusCodes.Status200OK)]
        [SwaggerOperation(
         Summary = "Get All My Subscriptions Endpoint")
        ]
        [HttpGet("my")]
        public async Task<IActionResult> GetMySubscriptions()
        {
            var userId = Guid.Parse(User.FindFirst("id")?.Value!);
            var subs = await _subscriptionService.GetUserSubscriptions(userId);
            return Ok(subs);
        }


        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [SwaggerOperation(
          Summary = "Subscribe To A Plan Endpoint",
            Description = "      Basic ===> 0,\r\n       Standard ===> 1,\r\n        Premium ===>2,")
         ]
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromQuery] PlanType planType, [FromQuery] bool autorenew)
        {
            var userId = User.FindFirst(ClaimTypes.Email)?.Value!;
            var result = await _subscriptionService.Subscribe(userId, planType, autorenew);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new BaseResponse() { Message = result.Message, Success = result.Success });
            }
        }

        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [SwaggerOperation(
          Summary = "Ugrade/Downgrade A Plan Endpoint",
            Description = "      Basic ===> 0,\r\n       Standard ===> 1,\r\n        Premium ===>2,")
         ]
        [HttpPut("upgrade")]
        public async Task<IActionResult> Upgrade([FromQuery] PlanType newPlanType, [FromQuery] bool autorenew)
        {
            var userId = User.FindFirst(ClaimTypes.Email)?.Value!;
            var result = await _subscriptionService.UpgradeOrDowngrade(userId, newPlanType, autorenew);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new BaseResponse() { Message = result.Message, Success = result.Success });
            }
        }


        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
          Summary = "Cancel A Subscription Endpoint")
         ]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.Email)?.Value!;
            var result = await _subscriptionService.CancelSubscription(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new BaseResponse() { Message = result.Message, Success = result.Success });
            }
        }

    }
}
