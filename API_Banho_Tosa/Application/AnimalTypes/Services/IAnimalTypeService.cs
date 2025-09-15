using API_Banho_Tosa.Application.AnimalTypes.DTOs;

namespace API_Banho_Tosa.Application.AnimalTypes.Services
{
    public interface IAnimalTypeService
    {
        Task<AnimalTypeResponse> CreateAnimalTypeAsync(AnimalTypeRequest request);
        Task<AnimalTypeResponse> GetAnimalTypeByIdAsync(int id);
        Task<IEnumerable<AnimalTypeResponse>> SearchAnimalTypesAsync(SearchAnimalTypeRequest searchParams);
        Task<AnimalTypeResponse> UpdateAnimalTypeAsync(int id, AnimalTypeRequest request);
        Task DeleteAnimalTypeAsync(int id, string? requestingIpAddress);
    }
}
