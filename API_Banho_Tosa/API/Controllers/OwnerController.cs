using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Application.Owners.Services;
using API_Banho_Tosa.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/owners")]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnerController(IOwnerService service)
        {
            _ownerService = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOwner([FromBody] OwnerRequest request)
        {
            var newOwner = await _ownerService.CreateOwnerAsync(request);
            return CreatedAtAction(
                actionName: nameof(GetOwnerByUuid), // nome da acao GET que busca um item
                routeValues: new { uuid = newOwner.Uuid }, // parametros da rota para acao GET
                value: newOwner // o objeto a ser retornado no corpo da resposta
            );
        }

        [HttpGet]
        public async Task<IActionResult> SearchOwners([FromQuery] SearchOwnerRequest searchParams)
        {
            var owners = await _ownerService.SearchOwnersAsync(searchParams);
            return Ok(owners);
        }

        [HttpGet("{uuid}")]
        public async Task<IActionResult> GetOwnerByUuid([FromRoute] Guid uuid)
        {
            var owner = await _ownerService.GetOwnerByUuid(uuid);
            return Ok(owner);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet("full_info")]
        public async Task<IActionResult> GetOwners()
        {
            var owners = await _ownerService.GetOwnersFullInfo();
            return Ok(owners);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet("archived")]
        public async Task<IActionResult> GetArchivedOwners()
        {
            var archivedOwners = await _ownerService.GetArchivedOwners();
            return Ok(archivedOwners);
        }

        [HttpPut("{uuid}")]
        public async Task<IActionResult> UpdateOwner([FromRoute] Guid uuid, [FromBody] OwnerRequest request)
        {
            var updatedOwner = await _ownerService.UpdateOwner(uuid, request);
            return Ok(updatedOwner);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPatch("{uuid}/reactivate")]
        public async Task<IActionResult> ReactivateOwner([FromRoute] Guid uuid)
        {
            var reactivatedOwner = await _ownerService.ReactivateOwner(uuid);
            return Ok(reactivatedOwner);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{uuid}")]
        public async Task<IActionResult> DeleteOwnerByUuid([FromRoute] Guid uuid)
        {
            await _ownerService.DeleteOwnerByUuid(uuid);
            return NoContent();
        }
    }
}
