using API_Banho_Tosa.Application.AnimalTypes.DTOs;
using API_Banho_Tosa.Application.AnimalTypes.Services;
using API_Banho_Tosa.Application.Breeds.Services;
using API_Banho_Tosa.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Banho_Tosa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/animal-types")]
    public class AnimalTypeController : ControllerBase
    {
        private readonly IAnimalTypeService _animalTypeService;
        private readonly IBreedService _breedService;
        private const string getTypeByIdRouteName = "GetAnimalTypeById";

        public AnimalTypeController(IAnimalTypeService animalTypeService, IBreedService breedService)
        {
            _animalTypeService = animalTypeService;
            _breedService = breedService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchAnimalTypesAsync([FromQuery] SearchAnimalTypeRequest searchParams)
        {
            var animalTypes = await _animalTypeService.SearchAnimalTypesAsync(searchParams);
            return Ok(animalTypes);
        }

        [HttpGet("{id}/breeds")]
        public async Task<IActionResult> GetBreedsForAnimalType([FromRoute] int id)
        {
            var breeds = await _breedService.GetBreedsByAnimalTypeIdAsync(id);
            return Ok(breeds);
        }

        [HttpGet("{id}", Name = getTypeByIdRouteName)]
        public async Task<IActionResult> GetAnimalTypeByIdAsync([FromRoute] int id)
        {
            var animalType = await _animalTypeService.GetAnimalTypeByIdAsync(id);
            return Ok(animalType);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAnimalTypeAsync([FromBody] AnimalTypeRequest request)
        {
            var createdEntity = await _animalTypeService.CreateAnimalTypeAsync(request);

            return CreatedAtRoute(
                routeName: getTypeByIdRouteName,
                routeValues: new { id = createdEntity.Id },
                value: createdEntity);
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimalTypeAsync([FromRoute] int id)
        {
            await _animalTypeService.DeleteAnimalTypeAsync(id);
            return NoContent();
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnimalTypeAsync([FromRoute] int id, [FromBody] AnimalTypeRequest request)
        {
            var updatedEntity = await _animalTypeService.UpdateAnimalTypeAsync(id, request);
            return Ok(updatedEntity);
        }
    }
}
