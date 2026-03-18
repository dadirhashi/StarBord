using Microsoft.AspNetCore.Mvc;
using StarBord.Services.IService;
using StarBord.DTOS;
namespace StarBord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthResponse _authResponse;

        public AuthController(IAuthResponse authResponse)
        {
            _authResponse = authResponse;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var authResponse = await _authResponse.LoginAsync(loginDto, cancellationToken);
            if (authResponse == null)
            {
                return Unauthorized();
            }
            return Ok(authResponse);
        }
    }
}
