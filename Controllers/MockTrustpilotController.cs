using Microsoft.AspNetCore.Mvc;

namespace StarBord.Controllers
{
    [ApiController]
    [Route("api/mock/trustpilot")]
    public class MockTrustpilotController : ControllerBase
    {
        // Trustpilot's hosted authorization page — for the mock, auto-approve and redirect back with a fake code
        [HttpGet("oauth/authorize")]
        public IActionResult Authorize(
            [FromQuery] string client_id,
            [FromQuery] string redirect_uri,
            [FromQuery] string state)
        {
            var code = "mock-auth-code-" + Guid.NewGuid().ToString("N")[..8];
            return Redirect($"{redirect_uri}?code={code}&state={state}");
        }

        // Exchange auth code for access + refresh tokens
        [HttpPost("oauth/oauth-business-users-for-applications/accesstoken")]
        public IActionResult ExchangeCode(
            [FromForm] string grant_type,
            [FromForm] string code,
            [FromForm] string redirect_uri)
        {
            return Ok(new
            {
                access_token = "mock-access-" + Guid.NewGuid().ToString("N"),
                refresh_token = "mock-refresh-" + Guid.NewGuid().ToString("N"),
                token_type = "Bearer",
                expires_in = 3600
            });
        }

        // Refresh an expired access token
        [HttpPost("oauth/oauth-business-users-for-applications/refresh")]
        public IActionResult Refresh(
            [FromForm] string grant_type,
            [FromForm] string refresh_token)
        {
            return Ok(new
            {
                access_token = "mock-access-" + Guid.NewGuid().ToString("N"),
                refresh_token = "mock-refresh-" + Guid.NewGuid().ToString("N"),
                token_type = "Bearer",
                expires_in = 3600
            });
        }

        // List reviews for a business unit
        [HttpGet("v1/private/business-units/{businessUnitId}/reviews")]
        public IActionResult GetReviews(string businessUnitId)
        {
            return Ok(new
            {
                reviews = new object[]
                {
                    new
                    {
                        id = "mock-tp-review-1",
                        stars = 5,
                        title = "Excellent service",
                        text = "Really impressed with the quality and speed.",
                        language = "en",
                        createdAt = DateTime.UtcNow.AddDays(-2),
                        consumer = new { displayName = "John D.", displayLocation = "Stockholm, SE" },
                        companyReply = (object?)null
                    },
                    new
                    {
                        id = "mock-tp-review-2",
                        stars = 3,
                        title = "Decent but room for improvement",
                        text = "Got what I needed but the wait was longer than expected.",
                        language = "en",
                        createdAt = DateTime.UtcNow.AddDays(-5),
                        consumer = new { displayName = "Anna S.", displayLocation = "Göteborg, SE" },
                        companyReply = (object?)null
                    }
                }
            });
        }

        // Post a reply to a review
        [HttpPost("v1/private/reviews/{reviewId}/reply")]
        public IActionResult ReplyToReview(string reviewId, [FromBody] MockReplyDto dto)
        {
            return Ok(new
            {
                reviewId,
                text = dto.Message,
                createdAt = DateTime.UtcNow
            });
        }

        public class MockReplyDto
        {
            public string Message { get; set; } = "";
        }
    }
}