using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBord.Application.Responses;
using StarBord.DTOS;
using System.Security.Claims;


namespace StarBord.Controllers;
[ApiController]
[Route("api/responses")]
[Authorize]
public class ResponsesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ResponsesController(IMediator mediator) => _mediator = mediator;

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<GetResponseDto>> CreateResponse(CreateResponseDto dto, CancellationToken cancellationToken)
    {
        var createResponse = await _mediator.Send(new RespondToReviewCommand(dto.ReviewId,CurrentUserId, dto), cancellationToken);
        return CreatedAtAction(nameof(GetResponse), new { responseId = createResponse.Id }, createResponse);
    }

    [HttpGet("{responseId:guid}")]
    public async Task<ActionResult<GetResponseDto>> GetResponse(Guid responseId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetResponseByIdQuery(responseId), cancellationToken);
        return response is null ? NotFound() : Ok(response);

    }

    [HttpGet("review/{reviewId:guid}")]
    public async Task<ActionResult<GetResponseDto>> GetResponseByReview(Guid reviewId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetResponseByReviewIdQuery(reviewId), cancellationToken);
        return response is null ? NotFound() : Ok(response);

    }

    [HttpPut("{responseId:guid}")]
    public async Task<ActionResult<GetResponseDto>> UpdateResponse(Guid responseId, CreateResponseDto dto, CancellationToken cancellationToken)
    {
        var updatedResponse = await _mediator.Send(new UpdateResponseCommand(responseId, CurrentUserId, dto), cancellationToken);
        return Ok(updatedResponse);
    }

    [HttpDelete("{deleteId:guid}")]
    public async Task<IActionResult> DeleteResponse(Guid deleteId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteResponseCommand(deleteId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

   
}