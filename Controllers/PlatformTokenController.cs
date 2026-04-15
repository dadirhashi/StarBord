using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBord.DTOS;
using StarBord.Services.IService;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlatformTokensController : ControllerBase
{
    private readonly IPlatformTokenService _platformTokenService;
    private readonly ILogger<PlatformTokensController> _logger;

    public PlatformTokensController(IPlatformTokenService platformTokenService, ILogger<PlatformTokensController> logger)
    {
        _platformTokenService = platformTokenService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlatformToken(CreatePlatformTokenDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _platformTokenService.CreatePlatformTokenAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetPlatformToken), new { id = token.Id }, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating platform token");
            return StatusCode(500, "An error occurred while creating the platform token");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlatformToken(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _platformTokenService.GetPlatformTokenByIdAsync(id, cancellationToken);
            if (token == null) return NotFound();
            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving platform token {TokenId}", id);
            return StatusCode(500, "An error occurred while retrieving the platform token");
        }
    }

    [HttpGet("business/{businessId}")]
    public async Task<IActionResult> GetPlatformTokensByBusiness(Guid businessId, CancellationToken cancellationToken)
    {
        try
        {
            var tokens = await _platformTokenService.GetPlatformTokensByBusinessIdAsync(businessId, cancellationToken);
            return Ok(tokens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving platform tokens for business {BusinessId}", businessId);
            return StatusCode(500, "An error occurred while retrieving platform tokens");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlatformToken(Guid id, CreatePlatformTokenDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _platformTokenService.UpdatePlatformTokenAsync(id, dto, cancellationToken);
            if (token == null) return NotFound();
            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating platform token {TokenId}", id);
            return StatusCode(500, "An error occurred while updating the platform token");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlatformToken(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _platformTokenService.DeletePlatformTokenAsync(id, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting platform token {TokenId}", id);
            return StatusCode(500, "An error occurred while deleting the platform token");
        }
    }
}
