using API_Banho_Tosa.Application.ServiceStatuses.DTOs;
using API_Banho_Tosa.Application.ServiceStatuses.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [Route("api/service-statuses")]
    [ApiController]
    public class ServiceStatusController(IServiceStatusService serviceStatusService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> SearchServiceStatus([FromQuery] ServiceStatusFilterQuery filter)
        {
            var response = await serviceStatusService.SearchServiceStatusesAsync(filter);
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateServiceStatus([FromRoute] int id, [FromBody] UpdateServiceStatusRequest request)
        {
            var response = await serviceStatusService.UpdateServiceStatusAsync(id, request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceStatus([FromBody] CreateServiceStatusRequest request)
        {
            var response = await serviceStatusService.CreateServiceStatusAsync(request);
            return CreatedAtRoute(
                routeName: nameof(GetServiceStatusById),
                routeValues: new { id = response.Id },
                value: response
            );
        }

        [HttpGet("{id:int}", Name = "GetServiceStatusById")]
        public async Task<IActionResult> GetServiceStatusById([FromRoute] int id)
        {
            var response = await serviceStatusService.GetServiceStatusByIdAsync(id);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteServiceStatus([FromRoute] int id)
        {
            await serviceStatusService.DeleteServiceStatusAsync(id);
            return NoContent();
        }
    }
}
