using API_Banho_Tosa.Application.Breeds.DTOs;
using API_Banho_Tosa.Application.Breeds.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.Breeds.Services
{
    public class BreedService : IBreedService
    {
        private readonly IBreedRepository _breedRepository;
        private readonly IAnimalTypeRepository _animalTypeRepository;

        public BreedService(IBreedRepository breedRepository, IAnimalTypeRepository animalTypeRepository)
        {
            this._breedRepository = breedRepository;
            this._animalTypeRepository = animalTypeRepository;
        }

        public async Task<BreedResponse> CreateBreedAsync(CreateBreedRequest request)
        {
            var animalTypeExists = await _animalTypeRepository.GetAnimalTypeByIdAsync(request.AnimalTypeId!.Value);

            if (animalTypeExists == null)
            {
                throw new ArgumentException($"The animal type with ID {request.AnimalTypeId} doesn't exists.");
            }

            var breed = Breed.Create(request.Name!, request.AnimalTypeId.Value);

            await _breedRepository.InsertBreedAsync(breed);
            await _breedRepository.SaveChangesAsync();

            var createdBreedWithDetails = await _breedRepository.GetBreedByIdAsync(breed.Id);

            if (createdBreedWithDetails == null)
            {
                throw new InvalidOperationException("Failed to fetch newly created breed.");
            }

            return createdBreedWithDetails.ToResponseDto();
        }

        public async Task DeleteBreedByIdAsync(int id)
        {
            var breedToDelete = await _breedRepository.GetBreedByIdAsync(id);

            if (breedToDelete == null)
            {
                throw new KeyNotFoundException($"Breed with ID {id} not found.");
            }

            _breedRepository.DeleteBreed(breedToDelete);
            await _breedRepository.SaveChangesAsync();
        }

        public async Task<BreedResponse> GetBreedByIdAsync(int id)
        {
            var breed = await _breedRepository.GetBreedByIdAsync(id);

            if (breed == null)
            {
                throw new KeyNotFoundException($"Breed with ID {id} not found.");
            }

            return breed.ToResponseDto();
        }

        public async Task<IReadOnlyCollection<BreedResponse>> SearchBreedsAsync(SearchBreedRequest searchParams)
        {
            var breeds = await _breedRepository.SearchBreedsAsync(searchParams);
            return breeds.ToColletionDto();
        }

        public async Task<BreedResponse> UpdateBreedAsync(int id, UpdateBreedRequest request)
        {
            var breedToUpdate = await _breedRepository.GetBreedByIdAsync(id);

            if (breedToUpdate == null)
            {
                throw new KeyNotFoundException($"Breed with ID {id} not found.");
            }

            breedToUpdate.UpdateName(request.Name);
            await _breedRepository.SaveChangesAsync();

            return breedToUpdate.ToResponseDto();
        }

        public async Task<IReadOnlyCollection<BreedResponse>> GetBreedsByAnimalTypeIdAsync(int animalTypeId)
        {
            if (animalTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(animalTypeId));
            }

            var breeds = await _breedRepository.GetBreedsByAnimalTypeIdAsync(animalTypeId);

            return breeds.ToColletionDto();
        }
    }
}
