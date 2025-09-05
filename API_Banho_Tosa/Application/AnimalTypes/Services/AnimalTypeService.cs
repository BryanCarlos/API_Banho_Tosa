using API_Banho_Tosa.Application.AnimalTypes.DTOs;
using API_Banho_Tosa.Application.AnimalTypes.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.AnimalTypes.Services
{
    public class AnimalTypeService : IAnimalTypeService
    {
        private readonly IAnimalTypeRepository _animalTypeRepository;

        public AnimalTypeService(IAnimalTypeRepository repository)
        {
            _animalTypeRepository = repository;
        }

        public async Task<AnimalTypeResponse> CreateAnimalTypeAsync(AnimalTypeRequest request)
        {
            var animalType = AnimalType.Create(request.Name);

            await _animalTypeRepository.InsertAnimalTypeAsync(animalType);
            await _animalTypeRepository.SaveChangesAsync();

            return animalType.ToResponseDto();
        }

        public async Task DeleteAnimalTypeAsync(int id)
        {
            var animalTypeToDelete = await _animalTypeRepository.GetAnimalTypeByIdAsync(id);

            if (animalTypeToDelete == null)
            {
                throw new KeyNotFoundException($"Animal type with ID {id} not found.");
            }

            _animalTypeRepository.DeleteAnimalType(animalTypeToDelete);
            await _animalTypeRepository.SaveChangesAsync();
        }

        public async Task<AnimalTypeResponse> GetAnimalTypeByIdAsync(int id)
        {
            var animalType = await _animalTypeRepository.GetAnimalTypeByIdAsync(id);

            if (animalType == null)
            {
                throw new KeyNotFoundException($"Animal type with ID {id} not found.");
            }

            return animalType.ToResponseDto();
        }

        public async Task<IEnumerable<AnimalTypeResponse>> SearchAnimalTypesAsync(SearchAnimalTypeRequest searchParams)
        {
            var animalTypes = await _animalTypeRepository.SearchAnimalTypesAsync(searchParams);
            return animalTypes.ToEnumerableResponseDto();
        }

        public async Task<AnimalTypeResponse> UpdateAnimalTypeAsync(int id, AnimalTypeRequest request)
        {
            var animalTypeToUpdate = await _animalTypeRepository.GetAnimalTypeByIdAsync(id);

            if (animalTypeToUpdate == null)
            {
                throw new KeyNotFoundException($"Animal type with ID {id} not found.");
            }

            animalTypeToUpdate.UpdateName(request.Name);
            await _animalTypeRepository.SaveChangesAsync();

            return animalTypeToUpdate.ToResponseDto();
        }
    }
}
