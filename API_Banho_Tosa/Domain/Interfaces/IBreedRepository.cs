using API_Banho_Tosa.Application.Breeds.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IBreedRepository
    {
        Task<IReadOnlyCollection<Breed>> SearchBreedsAsync(SearchBreedRequest searchParams);
        Task<IReadOnlyCollection<Breed>> GetBreedsByAnimalTypeIdAsync(int animalTypeId);
        Task<Breed?> GetBreedByIdAsync(int id);
        Task<int> SaveChangesAsync();
        Task InsertBreedAsync(Breed breed);
        void DeleteBreed(Breed breed);
    }
}
