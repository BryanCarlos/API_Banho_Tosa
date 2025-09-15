using API_Banho_Tosa.Application.Breeds.DTOs;
using API_Banho_Tosa.Application.Breeds.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_Banho_Tosa.API.Controllers
{
    [ApiController]
    [Route("/api/breeds")]
    public class BreedController : ControllerBase
    {
        private readonly IBreedService _breedService;
        private const string _getBreedByIdRouteName = "GetBreedById";

        public BreedController(IBreedService breedService)
        {
            _breedService = breedService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchBreedsAsync([FromQuery] SearchBreedRequest searchBreeds)
        {
            var breeds = await _breedService.SearchBreedsAsync(searchBreeds);
            return Ok(breeds);
        }

        [HttpGet("{id}", Name = _getBreedByIdRouteName)]
        public async Task<IActionResult> GetBreedByIdAsync([FromRoute] int id)
        {
            var breed = await _breedService.GetBreedByIdAsync(id);
            return Ok(breed);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBreedAsync([FromBody] CreateBreedRequest request)
        {
            var createdBreed = await _breedService.CreateBreedAsync(request);

            return CreatedAtRoute(
                routeName: _getBreedByIdRouteName,
                routeValues: new { id = createdBreed.Id },
                value: createdBreed
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBreedAsync([FromRoute] int id, [FromBody] UpdateBreedRequest request)
        {
            var updatedBreed = await _breedService.UpdateBreedAsync(id, request);
            return Ok(updatedBreed);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBreedAsync([FromRoute] int id)
        {
            var requestingIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _breedService.DeleteBreedByIdAsync(id, requestingIpAddress);
            return NoContent();
        }
    }
}
