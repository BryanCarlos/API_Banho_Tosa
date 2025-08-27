using API_Banho_Tosa.Application.AnimalTypes.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Domain.Interfaces
{
    public interface IAnimalTypeRepository
    {
        Task<IEnumerable<AnimalType>> SearchAnimalTypesAsync(SearchAnimalTypeRequest searchParams);
        Task<AnimalType?> GetAnimalTypeByIdAsync(int id);
        Task<int> SaveChangesAsync();
        Task InsertAnimalTypeAsync(AnimalType animalType);
        void DeleteAnimalType(AnimalType animalType);
    }
}
