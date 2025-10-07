using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Application.PetSizes.DTOs;
using API_Banho_Tosa.Application.PetSizes.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.PetSizes.Services
{
    public class PetSizeService : IPetSizeService
    {
        private readonly IPetSizeRepository _petSizeRepository;

        public PetSizeService(IPetSizeRepository petSizeRepository)
        {
            _petSizeRepository = petSizeRepository;
        }

        public async Task<PetSizeResponse> CreatePetSizeAsync(PetSizeRequest request)
        {
            var petSizeExists = await _petSizeRepository.GetPetSizeByDescriptionAsync(request.Description);

            if (petSizeExists != null)
            {
                throw new PetSizeAlreadyExistsException(request.Description);
            }

            var petSize = PetSize.Create(request.Description);
            _petSizeRepository.InsertPetSize(petSize);
            await _petSizeRepository.SaveChangesAsync();

            return petSize.ToResponse();
        }

        public async Task DeletePetSizeByIdAsync(int id)
        {
            var petSize = await _petSizeRepository.GetPetSizeByIdAsync(id);

            if (petSize == null)
            {
                throw new KeyNotFoundException($"Pet size with ID {id} not found.");
            }

            _petSizeRepository.DeletePetSize(petSize);
            await _petSizeRepository.SaveChangesAsync();
        }

        public async Task<PetSizeResponse?> GetPetSizeByIdAsync(int id)
        {
            var petSize = await _petSizeRepository.GetPetSizeByIdAsync(id);

            if (petSize == null)
            {
                throw new KeyNotFoundException($"Pet size with ID {id} not found.");
            }

            return petSize.ToResponse();
        }

        public async Task<IEnumerable<PetSizeResponse>> SearchPetSizesAsync(SearchPetSizeRequest searchParams)
        {
            var petSizes = await _petSizeRepository.SearchPetSizesAsync(searchParams);
            return petSizes.ToEnumerableResponse();
        }

        public async Task<PetSizeResponse> UpdatePetSizeAsync(int id, PetSizeRequest request)
        {
            var petSize = await _petSizeRepository.GetPetSizeByIdAsync(id);

            if (petSize == null)
            {
                throw new KeyNotFoundException($"Pet size with ID {id} not found.");
            }

            petSize.UpdateDescription(request.Description);
            await _petSizeRepository.SaveChangesAsync();

            return petSize.ToResponse();
        }
    }
}
