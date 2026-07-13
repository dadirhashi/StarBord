using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBord.Application.Reviews;
using StarBord.DTOS;

namespace StarBord.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<GetReviewDto>> CreateReview(
        CreateReviewDto createReviewDto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateReviewCommand(createReviewDto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { reviewId = result.Id }, result);
    }

    [HttpGet("business/{businessId:guid}")]
    public async Task<ActionResult<IEnumerable<GetReviewDto>>> GetByBusiness(
        Guid businessId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReviewsByBusinessIdQuery(businessId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{reviewId:guid}")]
    public async Task<ActionResult<GetReviewDto>> GetById(
        Guid reviewId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReviewByIdQuery(reviewId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{reviewId:guid}")]
    public async Task<ActionResult<GetReviewDto>> Update(
        Guid reviewId, CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateReviewCommand(reviewId, dto), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> Delete(Guid reviewId, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteReviewCommand(reviewId), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}