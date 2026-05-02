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
    public class BusinessesController : ControllerBase
    {
        private readonly IBusinessService _businessService;
        private readonly ILogger<BusinessesController> _logger;
        public BusinessesController(IBusinessService businessService, ILogger<BusinessesController> logger)
        {
            _businessService = businessService;
            _logger = logger;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
          var userId =Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
          var businesses = await _businessService.GetAllBusinessAsync(userId, cancellationToken);
          return Ok(businesses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
                var business = await _businessService.GetBusinessByIdAsync(id, cancellationToken);
                if (business == null)
                {
                    return NotFound();
                }
                return Ok(business);

        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBusinessDto businessDto, CancellationToken cancellationToken)
        {

                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var createdBusiness = await _businessService.CreateBusinessAsync(userId, businessDto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = createdBusiness.Id }, createdBusiness);
            
           
        }

        [HttpDelete("{id}")]
        public async Task <IActionResult> Delete (Guid id, CancellationToken cancellationToken)
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
    }
}
