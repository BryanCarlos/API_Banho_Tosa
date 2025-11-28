using API_Banho_Tosa.Application.PaymentStatuses.DTOs;
using API_Banho_Tosa.Application.PaymentStatuses.Services;
using API_Banho_Tosa.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payment-statuses")]
    public class PaymentStatusController(IPaymentStatusService paymentStatusService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> SearchPaymentStatuses([FromQuery] string? description)
        {
            var paymentStatuses = await paymentStatusService.SearchPaymentStatusesAsync(description);
            return Ok(paymentStatuses);
        }

        [HttpGet("{id:int}", Name = "GetPaymentStatusById")]
        public async Task<IActionResult> GetPaymentStatusById([FromRoute] int id)
        {
            var paymentStatus = await paymentStatusService.GetPaymentStatusByIdAsync(id);
            return Ok(paymentStatus);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreatePaymentStatus([FromBody] CreatePaymentStatusRequest request)
        {
            var paymentStatus = await paymentStatusService.CreatePaymentStatusAsync(request);
            return CreatedAtRoute(
                routeName: nameof(GetPaymentStatusById),
                routeValues: new { id = paymentStatus.Id },
                value: paymentStatus
            );
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePaymentStatus([FromRoute] int id, [FromBody] UpdatePaymentStatusRequest request)
        {
            var paymentStatus = await paymentStatusService.UpdatePaymentStatusAsync(id, request);
            return Ok(paymentStatus);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePaymentStatus([FromRoute] int id)
        {
            await paymentStatusService.DeletePaymentStatusByIdAsync(id);
            return NoContent();
        }
    }
}
