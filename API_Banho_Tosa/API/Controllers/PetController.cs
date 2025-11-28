using API_Banho_Tosa.Application.Pets.DTOs;
using API_Banho_Tosa.Application.Pets.Services;
using API_Banho_Tosa.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/pets")]
    public class PetController(IPetService petService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> SearchPets([FromQuery] PetFilterQuery filter)
        {
            var response = await petService.SearchPetsAsync(filter);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpGet("deleted")]
        public async Task<IActionResult> SearchDeletedPets([FromQuery] PetFilterQuery filter)
        {
            var response = await petService.SearchDeletedPetsAsync(filter);
            return Ok(response);
        }

        [HttpGet("{id:guid}", Name = "GetPetById")]
        public async Task<IActionResult> GetPetById([FromRoute] Guid id)
        {
            var response = await petService.GetPetByIdAsync(id);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePet([FromRoute] Guid id)
        {
            await petService.DeletePetAsync(id);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePet([FromRoute] Guid id, [FromBody] UpdatePetRequest request)
        {
            var response = await petService.UpdatePetAsync(id, request);
            return Ok(response);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPatch("{id:guid}/reactivate")]
        public async Task<IActionResult> ReactivatePet([FromRoute] Guid id)
        {
            await petService.ReactivatePetAsync(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePet([FromBody] CreatePetRequest request)
        {
            var response = await petService.CreatePetAsync(request);
            return CreatedAtRoute(
                routeName: nameof(GetPetById),
                routeValues: new { id = response.Id },
                value: response
            );
        }

        [HttpPost("{id:guid}/owners")]
        public async Task<IActionResult> SetNewOwner([FromRoute] Guid id, [FromBody] SetOwnerRequest request)
        {
            var response = await petService.SetNewOwnerAsync(id, request);
            return Ok(response);
        }

        [HttpDelete("{petId:guid}/owners/{ownerId:guid}")]
        public async Task<IActionResult> RemoveOwner([FromRoute] Guid petId, [FromRoute] Guid ownerId)
        {
            await petService.RemoveOwnerAsync(petId, ownerId);
            return NoContent();
        }
    }
}
