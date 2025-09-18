using API_Banho_Tosa.Application.Breeds.DTOs;

namespace API_Banho_Tosa.Application.Breeds.Services
{
    public interface IBreedService
    {
        Task<BreedResponse> CreateBreedAsync(CreateBreedRequest request);
        Task<BreedResponse> GetBreedByIdAsync(int id);
        Task<IReadOnlyCollection<BreedResponse>> SearchBreedsAsync(SearchBreedRequest searchParams);
        Task<BreedResponse> UpdateBreedAsync(int id, UpdateBreedRequest request);
        Task DeleteBreedByIdAsync(int id);
        Task<IReadOnlyCollection<BreedResponse>> GetBreedsByAnimalTypeIdAsync(int animalTypeId);
    }
}
