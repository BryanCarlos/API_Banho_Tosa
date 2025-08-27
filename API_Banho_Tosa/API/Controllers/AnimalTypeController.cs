using API_Banho_Tosa.Application.AnimalTypes.DTOs;
using API_Banho_Tosa.Application.AnimalTypes.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [ApiController]
    [Route("api/animal-types")]
    public class AnimalTypeController : ControllerBase
    {
        private readonly IAnimalTypeService _animalTypeService;
        private const string getTypeByIdRouteName = "GetAnimalTypeById";

        public AnimalTypeController(IAnimalTypeService service)
        {
            _animalTypeService = service;
        }

        [HttpGet]
        public async Task<IActionResult> SearchAnimalTypesAsync([FromQuery] SearchAnimalTypeRequest searchParams)
        {
            var animalTypes = await _animalTypeService.SearchAnimalTypesAsync(searchParams);
            return Ok(animalTypes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnimalTypeAsync([FromBody] AnimalTypeRequest request)
        {
            var createdEntity = await _animalTypeService.CreateAnimalTypeAsync(request);

            return CreatedAtRoute(
                routeName: getTypeByIdRouteName,
                routeValues: new { id = createdEntity.Id },
                value: createdEntity);
        }

        [HttpGet("{id}", Name = getTypeByIdRouteName)]
        public async Task<IActionResult> GetAnimalTypeByIdAsync([FromRoute] int id)
        {
            var animalType = await _animalTypeService.GetAnimalTypeByIdAsync(id);
            return Ok(animalType);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimalTypeAsync([FromRoute] int id)
        {
            await _animalTypeService.DeleteAnimalTypeAsync(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnimalTypeAsync([FromRoute] int id, [FromBody] AnimalTypeRequest request)
        {
            var updatedEntity = await _animalTypeService.UpdateAnimalTypeAsync(id, request);
            return Ok(updatedEntity);
        }
    }
}
