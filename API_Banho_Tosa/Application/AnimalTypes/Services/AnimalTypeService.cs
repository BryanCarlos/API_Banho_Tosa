using API_Banho_Tosa.Application.AnimalTypes.DTOs;
using API_Banho_Tosa.Application.AnimalTypes.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using System;

namespace API_Banho_Tosa.Application.AnimalTypes.Services
{
    public class AnimalTypeService : IAnimalTypeService
    {
        private readonly IAnimalTypeRepository _animalTypeRepository;
        private readonly ILogger<AnimalTypeService> _logger;

        public AnimalTypeService(IAnimalTypeRepository repository, ILogger<AnimalTypeService> logger)
        {
            _animalTypeRepository = repository;
            _logger = logger;
        }

        public async Task<AnimalTypeResponse> CreateAnimalTypeAsync(AnimalTypeRequest request)
        {
            _logger.LogInformation(
                "Attempting to create a new animal type with data: {@AnimalTypeRequest}",
                request
            );

            var animalType = AnimalType.Create(request.Name);

            await _animalTypeRepository.InsertAnimalTypeAsync(animalType);
            await _animalTypeRepository.SaveChangesAsync();

            _logger.LogInformation(
                "New animal type '{AnimalTypeName}' with ID {AnimalTypeId} was created.",
                animalType.Name,
                animalType.Id
            );

            return animalType.ToResponseDto();
        }

        public async Task DeleteAnimalTypeAsync(int id, string? requestingIpAddress)
        {
            var animalTypeToDelete = await _animalTypeRepository.GetAnimalTypeByIdAsync(id);

            if (animalTypeToDelete == null)
            {
                _logger.LogWarning("Attempted to delete an animal type with ID {AnimalTypeId} that was not found.", id);
                throw new KeyNotFoundException($"Animal type with ID {id} not found.");
            }

            _animalTypeRepository.DeleteAnimalType(animalTypeToDelete);
            await _animalTypeRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Animal type '{AnimalTypeName}' with ID {AnimalTypeId} deleted successfully by IP {RequestingIpAddress}.",
                animalTypeToDelete.Name,
                id,
                requestingIpAddress ?? "N/A"
            );
        }

        public async Task<AnimalTypeResponse> GetAnimalTypeByIdAsync(int id)
        {
            var animalType = await _animalTypeRepository.GetAnimalTypeByIdAsync(id);

            if (animalType == null)
            {
                _logger.LogWarning("Attempted to get an animal type info with ID {AnimalTypeId} that was not found.", id);
                throw new KeyNotFoundException($"Animal type with ID {id} not found.");
            }

            return animalType.ToResponseDto();
        }

        public async Task<IEnumerable<AnimalTypeResponse>> SearchAnimalTypesAsync(SearchAnimalTypeRequest searchParams)
        {
            var animalTypes = await _animalTypeRepository.SearchAnimalTypesAsync(searchParams);
            var animalTypesList = animalTypes.ToList();

            var criteria = new List<string>();
            var logArgs = new List<object>();

            if (!string.IsNullOrWhiteSpace(searchParams.Name))
            {
                criteria.Add("name like {SearchAnimalTypeName}");
                logArgs.Add(searchParams.Name);
            }

            if (searchParams.Id.HasValue)
            {
                criteria.Add("ID {SearchAnimalTypeId}");
                logArgs.Add(searchParams.Id);
            }

            logArgs.Add(animalTypesList.Count);

            string logMessage = string.Empty;
            if (criteria.Count == 0)
            {
                logMessage = "Search for all animal types found {AnimalTypesCount} results.";
            }
            else
            {
                logMessage = $"Search for animal types with {string.Join(" and ", criteria)} found {{AnimalTypesCount}} results.";
            }

            _logger.LogInformation(logMessage, logArgs.ToArray());

            return animalTypesList.ToEnumerableResponseDto();
        }

        public async Task<AnimalTypeResponse> UpdateAnimalTypeAsync(int id, AnimalTypeRequest request)
        {
            var animalTypeToUpdate = await _animalTypeRepository.GetAnimalTypeByIdAsync(id);

            if (animalTypeToUpdate == null)
            {
                _logger.LogWarning("Attempted to update an animal type info with ID {AnimalTypeId} that was not found.", id);
                throw new KeyNotFoundException($"Animal type with ID {id} not found.");
            }

            var oldData = new
            {
                Name = animalTypeToUpdate.Name
            };

            var newData = new
            {
                Name = request.Name
            };

            animalTypeToUpdate.UpdateName(request.Name);
            await _animalTypeRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Animal type {AnimalTypeName} with ID {AnimalTypeId} updated successfully. Changes {@Changes}",
                animalTypeToUpdate.Name,
                id,
                new { Old = oldData, New = newData }
            );

            return animalTypeToUpdate.ToResponseDto();
        }
    }
}
