using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBord.DTOS;
using StarBord.Services.IService;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResponsesController : ControllerBase
{
    private readonly IResponseService _responseService;
    private readonly ILogger<ResponsesController> _logger;

    public ResponsesController(IResponseService responseService, ILogger<ResponsesController> logger)
    {
        _responseService = responseService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateResponse(CreateResponseDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            var response = await _responseService.CreateResponseAsync(dto, userId.Value, cancellationToken);
            return CreatedAtAction(nameof(GetResponse), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating response");
            return StatusCode(500, "An error occurred while creating the response");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResponse(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _responseService.GetResponseByIdAsync(id, cancellationToken);
            if (response == null) return NotFound();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving response {ResponseId}", id);
            return StatusCode(500, "An error occurred while retrieving the response");
        }
    }

    [HttpGet("review/{reviewId}")]
    public async Task<IActionResult> GetResponseByReview(Guid reviewId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _responseService.GetResponseByReviewIdAsync(reviewId, cancellationToken);
            if (response == null) return NotFound();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving response for review {ReviewId}", reviewId);
            return StatusCode(500, "An error occurred while retrieving the response");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResponse(Guid id, CreateResponseDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _responseService.UpdateResponseAsync(id, dto, cancellationToken);
            if (response == null) return NotFound();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating response {ResponseId}", id);
            return StatusCode(500, "An error occurred while updating the response");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResponse(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _responseService.DeleteResponseAsync(id, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting response {ResponseId}", id);
            return StatusCode(500, "An error occurred while deleting the response");
        }
    }

    private Guid? GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
}