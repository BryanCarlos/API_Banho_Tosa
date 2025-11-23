using API_Banho_Tosa.Application.AvailableServices.DTOs;
using API_Banho_Tosa.Application.AvailableServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [Route("api/available-services")]
    [ApiController]
    public class AvailableServiceController(IAvailableServiceService availableServiceService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> SearchAvailableServices([FromQuery] AvailableServiceFilterQuery filter)
        {
            var response = await availableServiceService.SearchAvailableServicesAsync(filter);
            return Ok(response);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> SearchDeletedAvailableServices([FromQuery] AvailableServiceFilterQuery filter)
        {
            var response = await availableServiceService.SearchDeletedAvailableServicesAsync(filter);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAvailableService([FromBody] CreateAvailableServiceRequest request)
        {
            var response = await availableServiceService.CreateAvailableServiceAsync(request);
            return CreatedAtRoute(
                routeName: nameof(GetAvailableServiceByUuid),
                routeValues: new { id = response.Id },
                value: response
            );
        }

        [HttpGet("{id:guid}", Name = "GetAvailableServiceByUuid")]
        public async Task<IActionResult> GetAvailableServiceByUuid([FromRoute] Guid id)
        {
            var response = await availableServiceService.GetAvailableServiceByUuidAsync(id);
            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAvailableService([FromRoute] Guid id, [FromBody] UpdateAvailableServiceRequest request)
        {
            var response = await availableServiceService.UpdateAvailableServiceAsync(id, request);
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAvailableService([FromRoute] Guid id)
        {
            await availableServiceService.DeleteAvailableServiceAsync(id);
            return NoContent();
        }

        [HttpPatch("{id:guid}/reactivate")]
        public async Task<IActionResult> ReactivateAvailableService([FromRoute] Guid id)
        {
            await availableServiceService.ReactivateAvailableServiceAsync(id);
            return NoContent();
        }
    }
}
