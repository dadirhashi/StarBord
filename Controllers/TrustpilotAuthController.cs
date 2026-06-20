using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBord.Services;

namespace StarBord.Controllers
{
    [ApiController]
    [Route("api/trustpilot")]
    public class TrustpilotAuthController : ControllerBase
    {
        private readonly ITrustpilotService _trustpilot;
        private readonly ILogger<TrustpilotAuthController> _logger;
        private readonly IConfiguration _configuration;

        public TrustpilotAuthController(
            ITrustpilotService trustpilot,
            ILogger<TrustpilotAuthController> logger,
            IConfiguration configuration)
        {
            _trustpilot = trustpilot;
            _logger = logger;
            _configuration = configuration;
        }

        // Frontend calls this to get the URL it should redirect the user to
        [HttpGet("connect/{businessId:guid}")]
        [Authorize]
        public IActionResult Connect(Guid businessId)
        {
            // TODO: validate that the current user owns this business
            var url = _trustpilot.BuildAuthorizationUrl(businessId);
            return Ok(new { authorizationUrl = url });
        }

        // Trustpilot redirects the user's browser back here after they authorize
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(
     [FromQuery] string code,
     [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
                return BadRequest("Missing code or state");

            if (!Guid.TryParse(state, out var businessId))
                return BadRequest("Invalid state");

            var frontendUrl = _configuration["Frontend:BaseUrl"]
                ?? throw new InvalidOperationException("Frontend:BaseUrl not configured");

            try
            {
                await _trustpilot.HandleOAuthCallbackAsync(businessId, code);
                return Redirect($"{frontendUrl}/dashboard?trustpilot=connected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Trustpilot callback failed for {BusinessId}", businessId);
                return Redirect($"{frontendUrl}/dashboard?trustpilot=error");
            }
        }

        [HttpPost("sync/{businessId:guid}")]
        [Authorize]
        public async Task<IActionResult> Sync(Guid businessId)
        {
            try
            {
                var count = await _trustpilot.SyncReviewsAsync(businessId);
                return Ok(new { synced = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Trustpilot sync failed for {BusinessId}", businessId);
                return StatusCode(500, "Sync failed");
            }
        }
    }
}
