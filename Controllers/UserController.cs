using Microsoft.AspNetCore.Mvc;
using StarBord.Services.IService;
using StarBord.Models;
using StarBord.DTOS;
namespace StarBord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> GetAllUsers(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {

               _logger.LogError(ex, "An error occurred while retrieving users.");
                return StatusCode(500, "Server error occurred while retrieving users.");
            }
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<GetUserDto>> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email, cancellationToken);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user.");
                return StatusCode(500, "Server error occurred while retrieving the user.");
            }
        }

        [HttpPost]

        public async Task<ActionResult<User>> CreateNewUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(createUserDto, cancellationToken);

                // Return the created user with a 201 Created status code and a Location header pointing to the new resource
                return CreatedAtAction(nameof(GetUserByEmail), new { email = createdUser.Email }, createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the user.");
                return StatusCode(500, "Server error occurred while creating the user.");
            }

        }
    }
}
