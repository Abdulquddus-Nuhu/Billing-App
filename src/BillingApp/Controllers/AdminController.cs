using BillingApp.Application.Dtos.Responses;
using BillingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace BillingApp.Controllers
{
    [Route("api/admin")]
    [Authorize(Roles = "Admin" + "," + "SuperAdmin")]
    [SwaggerTag("Administrator Endpoints")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public AdminController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }


        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(IEnumerable<SubscribeResponse>), StatusCodes.Status200OK)]
        [SwaggerOperation(
          Summary = "Get All Subscriptions Endpoint",
          Description = "It requires Admin Privelage")
         ]
        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var subs = await _subscriptionService.GetAllSubscriptions();
            return Ok(subs);
        }


        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(IEnumerable<RevenueSummary>), StatusCodes.Status200OK)]
        [SwaggerOperation(
         Summary = "Get Revenue Summary Endpoint",
         Description = "It requires Admin Privelage")
        ]
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueSummary()
        {
            var summary = await _subscriptionService.GetRevenueSummaryAsync();
            return Ok(summary);
        }
    }
}
