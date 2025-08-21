using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Domain.ValueObjects;

namespace API_Banho_Tosa.Application.Owners
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnerService(IOwnerRepository repository)
        {
            this._ownerRepository = repository;
        }

        public async Task<OwnerResponse> CreateOwnerAsync(OwnerRequest dto)
        {
            var phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : PhoneNumber.Create(dto.Phone);
            var ownerEntity = Owner.Create(
                name: dto.Name, 
                phone: phone, 
                address: dto.Address);

            await _ownerRepository.InsertOwnerAsync(ownerEntity);
            await _ownerRepository.SaveChangesAsync();

            var responseDto = MapToResponse(ownerEntity);

            return responseDto;
        }

        public async Task<IEnumerable<OwnerResponse>> SearchOwnersAsync(SearchOwnerRequest requestParams)
        {
            var owners = await _ownerRepository.SearchOwnersAsync(requestParams);
            return MapToEnumerableResponse(owners);
        }

        public async Task <IEnumerable<OwnerResponseFullInfo>> GetOwnersFullInfo()
        {
            var owners = await _ownerRepository.GetOwnersAsync();
            return MapToEnumerableFullInfoResponse(owners);
        }

        public async Task DeleteOwnerByUuid(Guid uuid)
        {
            var ownerToDelete = await _ownerRepository.GetOwnerByUuidAsync(uuid);

            if (ownerToDelete == null)
            {
                throw new KeyNotFoundException($"Owner with UUID {uuid} not found.");
            }

            ownerToDelete.Delete();
            await _ownerRepository.SaveChangesAsync();
        }

        public async Task<OwnerResponse?> GetOwnerByUuid(Guid uuid)
        {
            var owner = await _ownerRepository.GetOwnerByUuidAsync(uuid);

            if (owner == null)
            {
                throw new KeyNotFoundException($"Owner with UUID {uuid} not found.");
            }

            return MapToResponse(owner);
        }

        public async Task<IEnumerable<OwnerResponseFullInfo>> GetArchivedOwners()
        {
            var archivedOwners = await _ownerRepository.GetArchivedOwnersAsync();
            return MapToEnumerableFullInfoResponse(archivedOwners);
        }

        public async Task<OwnerResponse> UpdateOwner(Guid uuid, OwnerRequest dto)
        {
            var owner = await _ownerRepository.GetOwnerByUuidAsync(uuid);

            if (owner == null)
            {
                throw new KeyNotFoundException($"Owner with UUID {uuid} not found.");
            }

            var phoneNumberObject = string.IsNullOrWhiteSpace(dto.Phone) ? null : PhoneNumber.Create(dto.Phone);

            owner.UpdateName(dto.Name);
            owner.UpdateAddress(dto.Address);
            owner.UpdatePhone(phoneNumberObject?.ToString());

            await _ownerRepository.SaveChangesAsync();
            return MapToResponse(owner);
        }

        public async Task<OwnerResponse> ReactivateOwner(Guid uuid)
        {
            var owner = await _ownerRepository.GetOwnerByUuidIgnoringFiltersAsync(uuid);

            if (owner == null)
            {
                throw new KeyNotFoundException($"Owner with UUID {uuid} not found.");
            }

            owner.Reactivate();
            await _ownerRepository.SaveChangesAsync();
            return MapToResponse(owner);
        }

        private IEnumerable<OwnerResponse> MapToEnumerableResponse(IEnumerable<Owner> owners)
        {
            return owners.Select(MapToResponse);
        }

        private OwnerResponse MapToResponse(Owner owner)
        {
            return new OwnerResponse
            {
                Uuid = owner.Uuid,
                Name = owner.Name,
                Address = owner.Address,
                Phone = owner.Phone?.Value,
                CreatedAt = owner.CreatedAt
            };
        }

        private IEnumerable<OwnerResponseFullInfo> MapToEnumerableFullInfoResponse(IEnumerable<Owner> owners)
        {
            return owners.Select(MapToFullInfoResponse);
        }

        private OwnerResponseFullInfo MapToFullInfoResponse(Owner owner)
        {
            return new OwnerResponseFullInfo
            {
                Uuid = owner.Uuid,
                Name = owner.Name,
                Address = owner.Address,
                Phone = owner.Phone?.Value,
                CreatedAt = owner.CreatedAt,
                UpdatedAt = owner.UpdatedAt,
                DeletedAt = owner.DeletedAt
            };
        }
    }
}
