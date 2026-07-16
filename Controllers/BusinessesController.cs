using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBord.Application.Businesses;
using StarBord.DTOS;
using System.Security.Claims;

namespace StarBord.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/businesses")]
    public class BusinessesController : ControllerBase
    {
       private readonly IMediator _mediator;

       public BusinessesController(IMediator mediator)
       {
           _mediator = mediator;
       }
        private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet]
        public async Task<ActionResult<List<GetBusinessDto>>> GetAllBusiness(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllBusinessByUserIdQuery(CurrentUserId), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{BusinessId:guid}")]
        public async Task<ActionResult<GetBusinessDto?>> GetBusinessById(Guid BusinessId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetBusinessByIdQuery(BusinessId), cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<GetBusinessDto>> CreateBusiness (CreateBusinessDto createBusinessDto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateBusinessCommand(CurrentUserId, createBusinessDto), cancellationToken);
            return CreatedAtAction(nameof(GetBusinessById), new { BusinessId = result.Id }, result);
        }

        [HttpPut("{BusinessId:guid}")]
        public async Task<ActionResult<GetBusinessDto?>> UpdateBusiness (Guid BusinessId, CreateBusinessDto createBusinessDto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateBusinessCommand(BusinessId, CurrentUserId, createBusinessDto), cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{BusinessId:guid}")]

        public async Task<IActionResult> DeleteBusiness (Guid BusinessId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteBusinessCommand(BusinessId, CurrentUserId), cancellationToken);
            return result ? NoContent() : NotFound();
        }



    }
}
