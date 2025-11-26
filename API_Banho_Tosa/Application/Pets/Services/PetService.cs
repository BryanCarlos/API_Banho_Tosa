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
        IOwnerRepository ownerRepository) : IPetService
    {
        public async Task<PetResponse> CreatePetAsync(CreatePetRequest request)
        {
            var breed = await breedRepository.GetBreedByIdAsync(request.BreedId);
            var petSize = await petSizeRepository.GetPetSizeByIdAsync(request.PetSizeId);
            var owner = await ownerRepository.GetOwnerByUuidAsync(request.OwnerId);

            var errors = new List<string>();

            if (breed is null)
            {
                errors.Add($"Breed with ID '{request.BreedId}' doesn't exist.");
            }
            if (petSize is null)
            {
                errors.Add($"Pet size with ID '{request.PetSizeId}' doesn't exist.");
            }
            if (owner is null)
            {
                errors.Add("Owner defined doesn't exist.");
            }

            if (errors.Count > 0)
            {
                throw new KeyNotFoundException(string.Join(" | ", errors));
            }

            var pet = Pet.Create(request.Name, request.BreedId, request.PetSizeId, request.BirthDate);
            pet.Owners.Add(owner!);

            petRepository.AddPet(pet);
            await petRepository.SaveChangesAsync();

            pet.SetBreed(breed!);
            pet.SetPetSize(petSize!);

            return pet.ToResponse();
        }

        public async Task<PetResponse> GetPetByIdAsync(Guid id)
        {
            var pet = await petRepository.GetPetByIdAsync(id);

            if (pet is null)
            {
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            return pet.ToResponse();
        }

        public async Task<PetResponse> UpdatePetAsync(Guid id, UpdatePetRequest request)
        {
            var pet = await petRepository.GetPetByIdAsync(id);

            if (pet is null)
            {
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            var errors = new List<string>();

            Breed? newBreed = null;
            PetSize? newPetSize = null;

            // --- FASE 1: VALIDAÇÃO (Apenas leitura) ---

            if (pet.BreedId != request.BreedId)
            {
                newBreed = await breedRepository.GetBreedByIdAsync(request.BreedId);
                if (newBreed is null) errors.Add($"Breed with ID '{request.BreedId}' doesn't exist.");
            }

            if (pet.PetSizeId != request.PetSizeId)
            {
                newPetSize = await petSizeRepository.GetPetSizeByIdAsync(request.PetSizeId);
                if (newPetSize is null) errors.Add($"Pet size with ID '{request.PetSizeId}' doesn't exist.");
            }

            if (errors.Count > 0) throw new KeyNotFoundException(string.Join(" | ", errors));

            // --- FASE 2: EXECUÇÃO (Alteração de estado) ---

            // Agora que sabemos que tudo é válido, aplicamos as mudanças
            if (newBreed != null) pet.SetBreed(newBreed);
            if (newPetSize != null) pet.SetPetSize(newPetSize);

            pet.Update(request.Name, request.BreedId, request.PetSizeId, request.LatestVisit, request.BirthDate);
            await petRepository.SaveChangesAsync();

            return pet.ToResponse();
        }

        public async Task DeletePetAsync(Guid id)
        {
            var pet = await petRepository.GetPetByIdAsync(id);

            if (pet is null)
            {
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            pet.Delete();
            await petRepository.SaveChangesAsync();
        }

        public async Task ReactivatePetAsync(Guid id)
        {
            var pet = await petRepository.GetDeletedPetByIdAsync(id);

            if (pet is null)
            {
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            pet.Reactivate();
            await petRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<PetResponse>> SearchPetsAsync(PetFilterQuery filter)
        {
            var pets = await petRepository.SearchPetsAsync(filter.PetName, filter.OwnerName, filter.OwnerId);
            return pets.Select(p => p.ToResponse());
        }

        public async Task<IEnumerable<PetResponse>> SearchDeletedPetsAsync(PetFilterQuery filter)
        {
            var pets = await petRepository.SearchDeletedPetsAsync(filter.PetName, filter.OwnerName, filter.OwnerId);
            return pets.Select(p => p.ToResponse());
        }

        public async Task<PetResponse> SetNewOwnerAsync(Guid id, SetOwnerRequest request)
        {
            var pet = await petRepository.GetPetByIdAsync(id);

            if (pet is null)
            {
                throw new KeyNotFoundException("Pet doesn't exist.");
            }

            var owner = await ownerRepository.GetOwnerByUuidAsync(request.OwnerId);

            if (owner is null)
            {
                throw new KeyNotFoundException("Defined owner doesn't exist.");
            }

            var ownerAlreadyLinked = pet.Owners.Any(o => o.Uuid == request.OwnerId);

            if (ownerAlreadyLinked)
            {
                throw new InvalidOperationException("Owner already linked.");
            }

            pet.Owners.Add(owner);
            await petRepository.SaveChangesAsync();

            return pet.ToResponse();
        }

        public async Task RemoveOwnerAsync(Guid petId, Guid ownerId)
        {
            var pet = await petRepository.GetPetByIdAsync(petId);

            if (pet is null) throw new KeyNotFoundException("Pet doesn't exist.");

            var ownerToRemove = pet.Owners.FirstOrDefault(o => o.Uuid == ownerId);

            if (ownerToRemove is null)
            {
                throw new KeyNotFoundException("This owner is not linked to this pet.");
            }

            pet.Owners.Remove(ownerToRemove);

            await petRepository.SaveChangesAsync();
        }
    }
}
