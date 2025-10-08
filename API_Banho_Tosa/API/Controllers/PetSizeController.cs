using API_Banho_Tosa.Application.PetSizes.DTOs;
using API_Banho_Tosa.Application.PetSizes.Services;
using API_Banho_Tosa.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [Authorize]
    [Route("api/pet-sizes")]
    [ApiController]
    public class PetSizeController : ControllerBase
    {
        private readonly IPetSizeService _petSizeService;
        private const string _getPetSizeByIdRouteName = "GetPetSizeById";

        public PetSizeController(IPetSizeService petSizeService)
        {
            _petSizeService = petSizeService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchPetSizesAsync([FromQuery] SearchPetSizeRequest searchParams)
        {
            var petSizes = await _petSizeService.SearchPetSizesAsync(searchParams);
            return Ok(petSizes);
        }

        [HttpGet("{id}", Name = _getPetSizeByIdRouteName)]
        public async Task<IActionResult> GetPetSizeByIdAsync([FromRoute] int id)
        {
            var petSize = await _petSizeService.GetPetSizeByIdAsync(id);
            return Ok(petSize);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreatePetSizeAsync([FromBody] PetSizeRequest request)
        {
            var response = await _petSizeService.CreatePetSizeAsync(request);
            return CreatedAtRoute(
                routeName: _getPetSizeByIdRouteName,
                routeValues: new { id = response.Id },
                value: response
            );
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePetSizeAsync([FromRoute] int id)
        {
            await _petSizeService.DeletePetSizeByIdAsync(id);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePetSizeAsync([FromRoute] int id, [FromBody] PetSizeRequest request)
        {
            var response = await _petSizeService.UpdatePetSizeAsync(id, request);
            return Ok(response);
        }
    }
}
