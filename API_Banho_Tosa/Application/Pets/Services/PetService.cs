using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Application.Pets.DTOs;
using API_Banho_Tosa.Application.Pets.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;

namespace API_Banho_Tosa.Application.Pets.Services
{
    public class PetService(
        IPetRepository petRepository,
        IBreedRepository breedRepository,
        IPetSizeRepository petSizeRepository,
        IOwnerRepository ownerRepository,
        ICurrentUserService currentUserService,
        ILogger<PetService> logger) : IPetService
    {
        public async Task<PetResponse> CreatePetAsync(CreatePetRequest request)
        {
            var breed = await breedRepository.GetBreedByIdAsync(request.BreedId);
            var petSize = await petSizeRepository.GetPetSizeByIdAsync(request.PetSizeId);
            var owner = await ownerRepository.GetOwnerByUuidAsync(request.OwnerId);

            var errors = new List<string>();
            var criteria = new List<string>();
            var logArgs = new List<string>();

            if (breed is null)
            {
                errors.Add($"Breed with ID '{request.BreedId}' doesn't exist.");
                logArgs.Add(request.BreedId.ToString());
                criteria.Add("breed with ID {BreedId}");
            }
            if (petSize is null)
            {
                errors.Add($"Pet size with ID '{request.PetSizeId}' doesn't exist.");
                logArgs.Add(request.PetSizeId.ToString());
                criteria.Add("pet size with ID {PetSizeId}");
            }
            if (owner is null)
            {
                errors.Add("Owner defined doesn't exist.");
                logArgs.Add(request.OwnerId.ToString());
                criteria.Add("owner with ID {OwnerId}");
            }

            if (errors.Count > 0)
            {
                logger.LogWarning($"Pet creation failed because {string.Join(" and ", criteria)} doesn't exist.", logArgs);
                throw new KeyNotFoundException(string.Join(" | ", errors));
            }

            var pet = Pet.Create(request.Name, request.BreedId, request.PetSizeId, request.BirthDate);
            pet.Owners.Add(owner!);

            petRepository.AddPet(pet);
            await petRepository.SaveChangesAsync();

            pet.SetBreed(breed!);
            pet.SetPetSize(petSize!);

            logger.LogInformation(
                "Pet {PetName} created linked with owner {OwnerName} (ID: {OwnerId}) by user {RequestingUserId} (Name: {RequestingUsername}).",
                pet.Name,
                owner!.Name,
                owner.Uuid,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );

            return pet.ToResponse();
        }

        public async Task<PetResponse> GetPetByIdAsync(Guid id)
        {
            var pet = await petRepository.GetPetByIdAsync(id);

            if (pet is null)
            {
                logger.LogWarning("Attempted to get pet info with ID {PetId} that was not found.", id);
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            return pet.ToResponse();
        }

        public async Task<PetResponse> UpdatePetAsync(Guid id, UpdatePetRequest request)
        {
            var pet = await petRepository.GetPetByIdAsync(id);

            if (pet is null)
            {
                logger.LogWarning("Attempted to update pet info with ID {PetId} that was not found.", id);
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            var errors = new List<string>();
            var criteria = new List<string>();
            var logArgs = new List<string>();

            Breed? newBreed = null;
            PetSize? newPetSize = null;

            if (pet.BreedId != request.BreedId)
            {
                newBreed = await breedRepository.GetBreedByIdAsync(request.BreedId);
                if (newBreed is null) 
                {
                    errors.Add($"Breed with ID '{request.BreedId}' doesn't exist.");
                    criteria.Add("breed with ID {BreedId}");
                    logArgs.Add(request.BreedId.ToString());
                }
            }

            if (pet.PetSizeId != request.PetSizeId)
            {
                newPetSize = await petSizeRepository.GetPetSizeByIdAsync(request.PetSizeId);
                if (newPetSize is null)
                {
                    errors.Add($"Pet size with ID '{request.PetSizeId}' doesn't exist.");
                    criteria.Add("pet size with ID {PetSizeId}");
                    logArgs.Add(request.PetSizeId.ToString());
                }
            }

            if (errors.Count > 0)
            {
                logger.LogWarning(
                    $"Cannot update pet {{PetName}} (ID: {{PetId}}) because {string.Join(" and ", criteria)} doesn't exist.",
                    pet.Name,
                    pet.Id,
                    logArgs
                );
                throw new KeyNotFoundException(string.Join(" | ", errors));
            }

            var oldData = new
            {
                Name = pet.Name,
                BreedId = pet.BreedId,
                PetSizeId = pet.PetSizeId,
                LatestVisit = pet.LatestVisit,
                BirthDate = pet.BirthDate
            };

            var newData = new
            {
                Name = request.Name,
                BreedId = request.BreedId,
                PetSizeId = request.PetSizeId,
                LatestVisit = request.LatestVisit,
                BirthDate = request.BirthDate
            };

            if (newBreed != null) pet.SetBreed(newBreed);
            if (newPetSize != null) pet.SetPetSize(newPetSize);

            pet.Update(request.Name, request.BreedId, request.PetSizeId, request.LatestVisit, request.BirthDate);
            await petRepository.SaveChangesAsync();


            logger.LogInformation(
                "Pet '{PetName}' with ID {PetId} updated successfully by user {RequestingUserId} (Name: {RequestingUserName}). Changes {@Changes}",
                pet.Name,
                id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A",
                new { Old = oldData, New = newData }
            );

            return pet.ToResponse();
        }

        public async Task DeletePetAsync(Guid id)
        {
            var pet = await petRepository.GetPetByIdAsync(id);

            if (pet is null)
            {
                logger.LogWarning("Attempted to delete pet with ID {PetId} that was not found.", id);
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            pet.Delete();
            await petRepository.SaveChangesAsync();

            logger.LogInformation(
                "Pet '{PetName}' with ID {PetId} deleted successfully by user {RequestingUserId} (Name: {RequestingUsername}).",
                pet.Name,
                id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );
        }

        public async Task ReactivatePetAsync(Guid id)
        {
            var pet = await petRepository.GetDeletedPetByIdAsync(id);

            if (pet is null)
            {
                logger.LogWarning("Attempted to reactivate pet with ID {PetId} that was not found.", id);
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            pet.Reactivate();
            await petRepository.SaveChangesAsync();

            logger.LogInformation(
                "Pet {PetName} (ID: {PetId} reactivated by user {RequestingUserId} (Name: {RequestingUsername}",
                pet.Name,
                pet.Id,
                currentUserService.UserId.ToString() ?? "N/A",
                currentUserService.Username ?? "N/A"
            );
        }

        public async Task<IEnumerable<PetResponse>> SearchPetsAsync(PetFilterQuery filter)
        {
            var pets = await petRepository.SearchPetsAsync(filter.PetName, filter.OwnerName, filter.OwnerId);

            var criteria = new List<string>();
            var logArgs = new List<object>();

            if (!string.IsNullOrWhiteSpace(filter.PetName))
            {
                logArgs.Add(filter.PetName);
                criteria.Add("name like {PetName}");
            }
            if (!string.IsNullOrWhiteSpace(filter.OwnerName))
            {
                logArgs.Add(filter.OwnerName);
                criteria.Add("owner name like {OwnerName}");
            }
            if (filter.OwnerId.HasValue)
            {
                logArgs.Add(filter.OwnerId.ToString()!);
                criteria.Add("owner ID like {OwnerId}");
            }

            var logMessage = string.Empty;
            if (criteria.Count == 0)
            {
                logMessage = "Search for all pets found {PetCount} results.";
                logArgs.Add(pets.Count().ToString());
            }
            else
            {
                logMessage = $"Search for pets with {string.Join(" and ", criteria)} found {{PetCount}}.";
                logArgs.Add(pets.Count().ToString());
            }

            logger.LogInformation(logMessage, logArgs);

            return pets.Select(p => p.ToResponse());
        }

        public async Task<IEnumerable<PetResponse>> SearchDeletedPetsAsync(PetFilterQuery filter)
        {
            var pets = await petRepository.SearchDeletedPetsAsync(filter.PetName, filter.OwnerName, filter.OwnerId);

            var criteria = new List<string>();
            var logArgs = new List<object>();

            if (!string.IsNullOrWhiteSpace(filter.PetName))
            {
                logArgs.Add(filter.PetName);
                criteria.Add("name like {PetName}");
            }
            if (!string.IsNullOrWhiteSpace(filter.OwnerName))
            {
                logArgs.Add(filter.OwnerName);
                criteria.Add("owner name like {OwnerName}");
            }
            if (filter.OwnerId.HasValue)
            {
                logArgs.Add(filter.OwnerId.ToString()!);
                criteria.Add("owner ID like {OwnerId}");
            }

            var logMessage = string.Empty;
            if (criteria.Count == 0)
            {
                logMessage = "Search for all deleted pets found {PetCount} results.";
                logArgs.Add(pets.Count().ToString());
            }
            else
            {
                logMessage = $"Search for deleted pets with {string.Join(" and ", criteria)} found {{PetCount}}.";
                logArgs.Add(pets.Count().ToString());
            }

            logger.LogInformation(logMessage, logArgs);

            return pets.Select(p => p.ToResponse());
        }

        public async Task<PetResponse> SetNewOwnerAsync(Guid id, SetOwnerRequest request)
        {
            var pet = await petRepository.GetPetByIdAsync(id);
            var owner = await ownerRepository.GetOwnerByUuidAsync(request.OwnerId);
            var ownerAlreadyLinked = pet?.Owners.Any(o => o.Uuid == request.OwnerId);

            var errors = new List<string>();
            var criteria = new List<string>();
            var logArgs = new List<string>();

            if (pet is null)
            {
                errors.Add("pet doesn't exist");
                criteria.Add("pet with ID {PetId} doesn't exist");
                logArgs.Add(id.ToString());
            }
            if (owner is null)
            {
                errors.Add("defined owner doesn't exist");
                criteria.Add("defined owner with ID {OwnerId} doesn't exist");
                logArgs.Add(request.OwnerId.ToString());
            }
            if (ownerAlreadyLinked.HasValue)
            {
                errors.Add("owner already linked");
                criteria.Add("owner already linked");
            }
            
            if (criteria.Count > 0)
            {
                var logMessage = $"Failed to set a new owner because {string.Join(" and ", criteria)}.";
                logger.LogWarning(logMessage, logArgs);
                throw new KeyNotFoundException(string.Join(" | ", errors));
            }

            pet!.Owners.Add(owner!);
            await petRepository.SaveChangesAsync();

            return pet.ToResponse();
        }

        public async Task RemoveOwnerAsync(Guid petId, Guid ownerId)
        {
            var pet = await petRepository.GetPetByIdAsync(petId);
            var ownerToRemove = pet?.Owners.FirstOrDefault(o => o.Uuid == ownerId);

            var errors = new List<string>();
            var criteria = new List<string>();
            var logArgs = new List<string>();

            if (pet is null)
            {
                errors.Add("Pet doesn't exist.");
                criteria.Add("pet with ID {PetId} doesn't exist");
                logArgs.Add(petId.ToString());
            }
            if (ownerToRemove is null)
            {
                errors.Add("This owner is not linked to this pet.");
                criteria.Add("owner with ID {OwnerId} is not linked to this pet");
                logArgs.Add(ownerId.ToString());
            }

            if (errors.Count > 0)
            {
                var logMessage = $"Failed to remove owner because {string.Join(" and ", criteria)}.";
                logger.LogWarning(logMessage, logArgs);

                throw new KeyNotFoundException(string.Join(" | ", errors));
            }

            pet!.Owners.Remove(ownerToRemove!);

            await petRepository.SaveChangesAsync();
        }
    }
}
