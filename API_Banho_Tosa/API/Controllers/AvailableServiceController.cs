using API_Banho_Tosa.Application.AvailableServices.DTOs;
using API_Banho_Tosa.Application.AvailableServices.Services;
using API_Banho_Tosa.Application.ServicePrices.DTOs;
using API_Banho_Tosa.Application.ServicePrices.Services;
using API_Banho_Tosa.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/available-services")]
    public class AvailableServiceController(
        IAvailableServiceService availableServiceService,
        IServicePriceService servicePriceService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> SearchAvailableServices([FromQuery] AvailableServiceFilterQuery filter)
        {
            var response = await availableServiceService.SearchAvailableServicesAsync(filter);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet("deleted")]
        public async Task<IActionResult> SearchDeletedAvailableServices([FromQuery] AvailableServiceFilterQuery filter)
        {
            var response = await availableServiceService.SearchDeletedAvailableServicesAsync(filter);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAvailableService([FromBody] CreateAvailableServiceRequest request)
        {
            var response = await availableServiceService.CreateAvailableServiceAsync(request);
            return CreatedAtRoute(
                routeName: nameof(GetAvailableServiceByUuid),
                routeValues: new { id = response.Uuid },
                value: response
            );
        }

        [HttpGet("{id:guid}", Name = "GetAvailableServiceByUuid")]
        public async Task<IActionResult> GetAvailableServiceByUuid([FromRoute] Guid id)
        {
            var response = await availableServiceService.GetAvailableServiceByUuidAsync(id);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAvailableService([FromRoute] Guid id, [FromBody] UpdateAvailableServiceRequest request)
        {
            var response = await availableServiceService.UpdateAvailableServiceAsync(id, request);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAvailableService([FromRoute] Guid id)
        {
            await availableServiceService.DeleteAvailableServiceAsync(id);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPatch("{id:guid}/reactivate")]
        public async Task<IActionResult> ReactivateAvailableService([FromRoute] Guid id)
        {
            await availableServiceService.ReactivateAvailableServiceAsync(id);
            return NoContent();
        }

        [HttpGet("{id:guid}/prices")]
        public async Task<IActionResult> GetPricesByService([FromRoute] Guid id)
        {
            var response = await servicePriceService.GetServicePricesByServiceAsync(id);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("{id:guid}/prices")]
        public async Task<IActionResult> AddServicePrice([FromRoute] Guid id, [FromBody] AddServicePriceRequest request)
        {
            var response = await servicePriceService.AddServicePriceAsync(id, request);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id:guid}/prices/{petSizeId:int}")]
        public async Task<IActionResult> DeleteServicePrice([FromRoute] Guid id, [FromRoute] int petSizeId)
        {
            await servicePriceService.DeleteServicePriceAsync(id, petSizeId);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.Admin)] 
        [HttpPut("{id:guid}/prices/{petSizeId:int}")]
        public async Task<IActionResult> UpdateServicePrice([FromRoute] Guid id, [FromRoute] int petSizeId, [FromBody] UpdatePriceRequest request)
        {
            var response = await servicePriceService.UpdateServicePriceAsync(id, petSizeId, request);
            return Ok(response);
        }
    }
}
