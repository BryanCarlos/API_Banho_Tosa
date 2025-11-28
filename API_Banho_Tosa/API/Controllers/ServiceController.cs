using API_Banho_Tosa.Application.Services.DTOs;
using API_Banho_Tosa.Application.Services.Services;
using API_Banho_Tosa.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/services")]
    public class ServiceController(IServiceService serviceService) : ControllerBase
    {
        private const string GetServiceByIdRoute = "GetServiceById";

        [HttpGet]
        public async Task<IActionResult> SearchServices([FromQuery] ServiceFilterQuery filter)
        {
            var response = await serviceService.SearchServicesAsync(filter);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet("deleted")]
        public async Task<IActionResult> SearchDeletedServices([FromQuery] ServiceFilterQuery filter)
        {
            var response = await serviceService.SearchDeletedServicesAsync(filter);
            return Ok(response);
        }

        [HttpGet("{id:guid}", Name = GetServiceByIdRoute)]
        public async Task<IActionResult> GetServiceById([FromRoute] Guid id)
        {
            var response = await serviceService.GetServiceByUuidAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
        {
            var response = await serviceService.CreateServiceAsync(request);

            return CreatedAtRoute(
                routeName: GetServiceByIdRoute,
                routeValues: new { id = response.Id },
                value: response
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateService([FromRoute] Guid id, [FromBody] UpdateServiceRequest request)
        {
            var response = await serviceService.UpdateServiceAsync(id, request);
            return Ok(response);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateServiceStatus([FromRoute] Guid id, [FromBody] UpdateServiceServiceStatusRequest request)
        {
            var response = await serviceService.UpdateServiceStatusAsync(id, request.StatusId);
            return Ok(response);
        }

        [HttpPatch("{id:guid}/payment-status")]
        public async Task<IActionResult> UpdatePaymentStatus([FromRoute] Guid id, [FromBody] UpdateServicePaymentStatusRequest request)
        {
            var response = await serviceService.UpdatePaymentStatusAsync(id, request.PaymentStatusId);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteService([FromRoute] Guid id)
        {
            await serviceService.DeleteServiceAsync(id);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPatch("{id:guid}/reactivate")]
        public async Task<IActionResult> ReactivateService([FromRoute] Guid id)
        {
            await serviceService.ReactivateServiceAsync(id);
            return NoContent();
        }
    }
}
