using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBord.Services.IService;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GoogleReviewsController : ControllerBase
{
    private readonly IGoogleReviewService _googleReviewService;
    private readonly ILogger<GoogleReviewsController> _logger;

    public GoogleReviewsController(IGoogleReviewService googleReviewService, ILogger<GoogleReviewsController> logger)
    {
        _googleReviewService = googleReviewService;
        _logger = logger;
    }

    [HttpPost("fetch/{businessId}")]
    public async Task<IActionResult> FetchReviews(Guid businessId, CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _googleReviewService.FetchReviewAsync(businessId, cancellationToken);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Google reviews for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while fetching Google reviews");
        }
    }
}