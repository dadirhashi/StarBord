using Microsoft.AspNetCore.Mvc;
using StarBord.DTOS;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using StarBord.Application.Users;
namespace StarBord.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;   

        public UserController (IMediator mediator) => _mediator = mediator;


        [HttpGet]
        public async Task<ActionResult<List<GetUserDto>>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
            return Ok(users);
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<GetUserDto?>> GetUserByEmail( string email, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new GetUserByEmailQuery(email), cancellationToken);
            return user is null ? NotFound() : Ok(user);


        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<GetUserDto>> CreateNewUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new CreateUserCommand(createUserDto), cancellationToken);
            return CreatedAtAction(nameof(GetUserByEmail), new { email = user.Email }, user);
        }

    }
}
