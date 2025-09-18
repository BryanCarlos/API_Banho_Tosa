using API_Banho_Tosa.Application.Breeds.DTOs;
using API_Banho_Tosa.Application.Breeds.Mappers;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.Breeds.Services
{
    public class BreedService : IBreedService
    {
        private readonly IBreedRepository _breedRepository;
        private readonly IAnimalTypeRepository _animalTypeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<BreedService> _logger;

        public BreedService(IBreedRepository breedRepository, IAnimalTypeRepository animalTypeRepository, ICurrentUserService currentUserService, ILogger<BreedService> logger)
        {
            this._breedRepository = breedRepository;
            this._animalTypeRepository = animalTypeRepository;
            this._currentUserService = currentUserService;
            this._logger = logger;
        }

        public async Task<BreedResponse> CreateBreedAsync(CreateBreedRequest request)
        {
            _logger.LogInformation(
                "Attempting to create a new breed with data: {@CreateBreedRequest}.",
                request
            );

            var animalTypeExists = await _animalTypeRepository.GetAnimalTypeByIdAsync(request.AnimalTypeId!.Value);

            if (animalTypeExists == null)
            {
                _logger.LogWarning(
                    "Failed to create breed. The animal type with ID {AnimalTypeId} doesn't exist.",
                    request.AnimalTypeId
                );
                throw new ArgumentException($"The animal type with ID {request.AnimalTypeId} doesn't exists.");
            }

            var breed = Breed.Create(request.Name!, request.AnimalTypeId.Value);

            await _breedRepository.InsertBreedAsync(breed);
            await _breedRepository.SaveChangesAsync();

            _logger.LogInformation(
                "New breed '{BreedName}' (ID: {BreedId}) created successfully by user {RequestingUserId} (Name: {RequestingUsername}).",
                breed.Name,
                breed.Id,
                _currentUserService.UserId.ToString() ?? "N/A",
                _currentUserService.Username ?? "N/A"
            );

            var createdBreedWithDetails = await _breedRepository.GetBreedByIdAsync(breed.Id);

            if (createdBreedWithDetails == null)
            {
                _logger.LogError("Critical error: Failed to fetch newly created breed with ID {BreedId}.", breed.Id);
                throw new InvalidOperationException("Failed to fetch newly created breed.");
            }

            return createdBreedWithDetails.ToResponseDto();
        }

        public async Task DeleteBreedByIdAsync(int id)
        {
            var breedToDelete = await _breedRepository.GetBreedByIdAsync(id);

            if (breedToDelete == null)
            {
                _logger.LogWarning("Attempted to delete breed with ID {BreedId} that was not found.", id);
                throw new KeyNotFoundException($"Breed with ID {id} not found.");
            }

            _breedRepository.DeleteBreed(breedToDelete);
            await _breedRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Breed '{BreedName}' (ID: {BreedId}) deleted successfully by  by user {RequestingUserId} (Name: {RequestingUsername}).",
                breedToDelete.Name,
                id,
                _currentUserService.UserId.ToString() ?? "N/A",
                _currentUserService.Username ?? "N/A"
            );
        }

        public async Task<BreedResponse> GetBreedByIdAsync(int id)
        {
            var breed = await _breedRepository.GetBreedByIdAsync(id);

            if (breed == null)
            {
                _logger.LogWarning("Attempted to get breed info with ID {BreedId} that was not found.", id);
                throw new KeyNotFoundException($"Breed with ID {id} not found.");
            }

            return breed.ToResponseDto();
        }

        public async Task<IReadOnlyCollection<BreedResponse>> SearchBreedsAsync(SearchBreedRequest searchParams)
        {
            var breeds = await _breedRepository.SearchBreedsAsync(searchParams);
            var breedsList = breeds.ToList();

            var criteria = new List<string>();
            var logArgs = new List<object>();

            if (!string.IsNullOrWhiteSpace(searchParams.Name))
            {
                criteria.Add("name like {SearchBreedName}");
                logArgs.Add(searchParams.Name);
            }

            if (searchParams.Id.HasValue)
            {
                criteria.Add("ID {SearchBreedId}");
                logArgs.Add(searchParams.Id);
            }

            logArgs.Add(breedsList.Count);

            string logMessage;
            if (criteria.Count == 0)
            {
                logMessage = "Search for all breeds found {BreedsCount} results.";
            }
            else
            {
                logMessage = $"Search for breeds with {string.Join(" and ", criteria)} found {{BreedsCount}} results.";
            }

            _logger.LogInformation(logMessage, logArgs.ToArray());

            return breedsList.ToColletionDto();
        }

        public async Task<BreedResponse> UpdateBreedAsync(int id, UpdateBreedRequest request)
        {
            var breedToUpdate = await _breedRepository.GetBreedByIdAsync(id);

            if (breedToUpdate == null)
            {
                _logger.LogWarning("Attempted to update breed with ID {BreedId} that was not found.", id);
                throw new KeyNotFoundException($"Breed with ID {id} not found.");
            }

            var oldData = new { Name = breedToUpdate.Name };
            var newData = new { Name = request.Name };

            breedToUpdate.UpdateName(request.Name);
            await _breedRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Breed '{OriginalBreedName}' (ID: {BreedId}) updated successfully by user {RequestingUserId} (Name: {RequestingUsername}). Changes: {@Changes}",
                oldData.Name,
                id,
                _currentUserService.UserId.ToString() ?? "N/A",
                _currentUserService.Username ?? "N/A",
                new { Old = oldData, New = newData }
            );

            return breedToUpdate.ToResponseDto();
        }

        public async Task<IReadOnlyCollection<BreedResponse>> GetBreedsByAnimalTypeIdAsync(int animalTypeId)
        {
            if (animalTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(animalTypeId));
            }

            var breeds = await _breedRepository.GetBreedsByAnimalTypeIdAsync(animalTypeId);
            var breedsList = breeds.ToList();

            _logger.LogInformation(
                "Search for breeds with AnimalTypeId {AnimalTypeId} found {BreedsCount} results.",
                animalTypeId,
                breedsList.Count
            );

            return breeds.ToColletionDto();
        }
    }
}
