using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarBord.DTOS;
using StarBord.Services.IService;
using System.Linq;
using System.Security.Claims;

namespace StarBord.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class BusinesseController : ControllerBase
    {
        private readonly IBusinessService _businessService;
        private readonly ILogger<BusinesseController> _logger;
        public BusinesseController(IBusinessService businessService, ILogger<BusinesseController> logger)
        {
            _businessService = businessService;
            _logger = logger;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var busnisse = await _businessService.GetAllBusinessAsync(cancellationToken);
                return Ok(busnisse);

              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching businesses.");
                return StatusCode(500, "An unexpected error occurred.");

            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var business = await _businessService.GetBusinessByIdAsync(id, cancellationToken);
                if (business == null)
                {
                    return NotFound();
                }
                return Ok(business);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching business with ID: {id}");
                return StatusCode(500, "An unexpected error occurred.");

            }

        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBusinessDto businessDto, CancellationToken cancellationToken)
        {
            try
            {
                //foreach (var claim in User.Claims)
                //{
                //    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                //}

                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var createdBusiness = await _businessService.CreateBusinessAsync(userId, businessDto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = createdBusiness.Id }, createdBusiness);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a business.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{id}")]
        public async Task <IActionResult> Delete (Guid id, CancellationToken cancellationToken)
        {
            try
            {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _businessService.DeleteBusinessAsync(id, userId, cancellationToken);
                
                if (!result)
                {
                    Console.WriteLine($"Failed to delete business with ID: {id}");
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting business with ID: {id}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
