using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBord.DTOS;
using StarBord.Services.IService;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview(CreateReviewDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewService.CreateReviewAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating review");
            return StatusCode(500, "An error occurred while creating the review");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewService.GetReviewByIdAsync(id, cancellationToken);
            if (review == null) return NotFound();
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review {ReviewId}", id);
            return StatusCode(500, "An error occurred while retrieving the review");
        }
    }

    [HttpGet("business/{businessId}")]
    public async Task<IActionResult> GetReviewsByBusiness(Guid businessId, CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _reviewService.GetReviewsByBusinessIdAsync(businessId, cancellationToken);
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reviews for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while retrieving reviews");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(Guid id, CreateReviewDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewService.UpdateReviewAsync(id, dto, cancellationToken);
            if (review == null) return NotFound();
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review {ReviewId}", id);
            return StatusCode(500, "An error occurred while updating the review");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _reviewService.DeleteReviewAsync(id, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review {ReviewId}", id);
            return StatusCode(500, "An error occurred while deleting the review");
        }
    }
}