using API_Banho_Tosa.API.Services;
using API_Banho_Tosa.Application.Common.Exceptions;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Application.PetSizes.DTOs;
using API_Banho_Tosa.Application.PetSizes.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Infrastructure.Persistence.Repositories;

namespace API_Banho_Tosa.Application.PetSizes.Services
{
    public class PetSizeService : IPetSizeService
    {
        private readonly IPetSizeRepository _petSizeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<PetSizeService> _logger;

        public PetSizeService(
            IPetSizeRepository petSizeRepository,
            ICurrentUserService currentUserService,
            ILogger<PetSizeService> logger
            )
        {
            _petSizeRepository = petSizeRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PetSizeResponse> CreatePetSizeAsync(PetSizeRequest request)
        {
            var petSizeExists = await _petSizeRepository.GetPetSizeByDescriptionAsync(request.Description);

            if (petSizeExists != null)
            {
                _logger.LogWarning("Attempting to create a pet size {PetSizeDescription} that already exist.", request.Description.Trim().ToUpper());
                throw new PetSizeAlreadyExistsException(request.Description);
            }

            var petSize = PetSize.Create(request.Description);
            _petSizeRepository.InsertPetSize(petSize);
            await _petSizeRepository.SaveChangesAsync();

            _logger.LogInformation(
                "New pet size '{PetSizeName}' with ID {PetSizeId} was created by {RequestingUserId} (Name: {RequestingUsername}",
                petSize.Description,
                petSize.Id,
                _currentUserService.UserId.ToString() ?? "N/A",
                _currentUserService.Username ?? "N/A"
            );

            return petSize.ToResponse();
        }

        public async Task DeletePetSizeByIdAsync(int id)
        {
            var petSize = await _petSizeRepository.GetPetSizeByIdAsync(id);

            if (petSize == null)
            {
                _logger.LogWarning("Attempted to delete pet size with ID {PetSizeId} that was not found.", id);
                throw new KeyNotFoundException($"Pet size with ID {id} not found.");
            }

            _petSizeRepository.DeletePetSize(petSize);
            await _petSizeRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Pet size '{PetSizeName}' with ID {PetSizeId} deleted successfully by user {RequestingUserId} (Name: {RequestingUsername}).",
                petSize.Description,
                id,
                _currentUserService.UserId.ToString() ?? "N/A",
                _currentUserService.Username ?? "N/A"
            );
        }

        public async Task<PetSizeResponse?> GetPetSizeByIdAsync(int id)
        {
            var petSize = await _petSizeRepository.GetPetSizeByIdAsync(id);

            if (petSize == null)
            {
                _logger.LogWarning("Attempted to get pet size info with ID {PetSizeId} that was not found.", id);
                throw new KeyNotFoundException($"Pet size with ID {id} not found.");
            }

            return petSize.ToResponse();
        }

        public async Task<IEnumerable<PetSizeResponse>> SearchPetSizesAsync(SearchPetSizeRequest searchParams)
        {
            var petSizes = await _petSizeRepository.SearchPetSizesAsync(searchParams);

            var criteria = new List<string>();
            var logArgs = new List<string>();

            if (searchParams.Id.HasValue)
            {
                criteria.Add("pet size with ID {PetSizeId}");
                logArgs.Add(searchParams.Id.Value.ToString());
            }
            if (!string.IsNullOrWhiteSpace(searchParams.Description))
            {
                criteria.Add("pet size description like {PetSizeDescription}");
                logArgs.Add(searchParams.Description);
            }

            var logMessage = string.Empty;

            if (criteria.Count == 0)
            {
                logMessage = "Search for all pet sizes retuned {PetSizeCount} results.";
                logArgs.Add(petSizes.Count().ToString());
            }
            else
            {
                logMessage = $"Search for pet sizes with {string.Join(" and ", criteria)} retuned {{PetSizeCount}} results.";
                logArgs.Add(petSizes.Count().ToString());
            }

            _logger.LogInformation(logMessage, logArgs);

            return petSizes.ToEnumerableResponse();
        }

        public async Task<PetSizeResponse> UpdatePetSizeAsync(int id, PetSizeRequest request)
        {
            var petSize = await _petSizeRepository.GetPetSizeByIdAsync(id);

            if (petSize == null)
            {
                _logger.LogWarning("Attempted to update pet size info with ID {PetSizeId} that was not found.", id);
                throw new KeyNotFoundException($"Pet size with ID {id} not found.");
            }

            var oldData = new
            {
                Description = petSize.Description
            };

            var newData = new
            {
                Description = request.Description
            };

            petSize.UpdateDescription(request.Description);
            await _petSizeRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Pet size {PetSizeName} with ID {PetSizeId} updated successfully by user {RequestingUserId} (Name: {RequestingUserName}). Changes {@Changes}",
                petSize.Description,
                id,
                _currentUserService.UserId.ToString() ?? "N/A",
                _currentUserService.Username ?? "N/A",
                new { Old = oldData, New = newData }
            );


            return petSize.ToResponse();
        }
    }
}
