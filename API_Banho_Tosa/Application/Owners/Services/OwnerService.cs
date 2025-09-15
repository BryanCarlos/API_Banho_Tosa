using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Application.Owners.Mappers;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Domain.ValueObjects;

namespace API_Banho_Tosa.Application.Owners.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ILogger<OwnerService> _logger;

        public OwnerService(IOwnerRepository repository, ILogger<OwnerService> logger)
        {
            _ownerRepository = repository;
            _logger = logger;
        }

        public async Task<OwnerResponse> CreateOwnerAsync(OwnerRequest dto)
        {
            _logger.LogInformation(
                "Attempting to create a new owner with data: {@OwnerRequest}",
                dto
            );

            var phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : PhoneNumber.Create(dto.Phone);
            var ownerEntity = Owner.Create(
                name: dto.Name, 
                phone: phone, 
                address: dto.Address);

            await _ownerRepository.InsertOwnerAsync(ownerEntity);
            await _ownerRepository.SaveChangesAsync();

            var newOwner = ownerEntity.MapToResponse();

            _logger.LogInformation(
                "New owner {OwnerName} created with UUID {OwnerUuid}.",
                newOwner.Name, 
                newOwner.Uuid
            );

            return newOwner;
        }

        public async Task<IEnumerable<OwnerResponse>> SearchOwnersAsync(SearchOwnerRequest requestParams)
        {
            var owners = await _ownerRepository.SearchOwnersAsync(requestParams);
            var ownersList = owners.ToList();

            var criteria = new List<string>();
            var logArgs = new List<object>();

            if (!string.IsNullOrWhiteSpace(requestParams.Name))
            {
                criteria.Add("name like {SearchOwnerName}");
                logArgs.Add(requestParams.Name);
            }

            if (!string.IsNullOrWhiteSpace(requestParams.Phone))
            {
                criteria.Add("phone {SearchOwnerPhone}");
                logArgs.Add(requestParams.Phone);
            }

            logArgs.Add(ownersList.Count);

            string logMessage = string.Empty;
            if (criteria.Count == 0)
            {
                logMessage = "Search for all owners found {OwnersFoundCount} results.";
            }
            else
            {
                logMessage = $"Search for owners with {string.Join(" and ", criteria)} found {{OwnersFoundCount}} results.";
            }

            _logger.LogInformation(logMessage, logArgs.ToArray());

            return ownersList.MapToEnumerableResponse();
        }

        public async Task <IEnumerable<OwnerResponseFullInfo>> GetOwnersFullInfo(string? requestingIpAddress)
        {
            _logger.LogInformation(
                "IP {RequestingIpAddress} is searching for full info from all owners",
                requestingIpAddress ?? "N/A"
            );

            var owners = await _ownerRepository.GetOwnersAsync();
            var ownersList = owners.ToList();

            _logger.LogInformation(
                "Search from IP {RequestingIpAddress} found {OwnersFoundCount} owners with full info.",
                requestingIpAddress ?? "N/A",
                ownersList.Count
            );

            return ownersList.MapToEnumerableFullInfoResponse();
        }

        public async Task DeleteOwnerByUuid(Guid uuid, string? requestingIpAddress)
        {
            var ownerToDelete = await _ownerRepository.GetOwnerByUuidAsync(uuid);

            if (ownerToDelete == null)
            {
                _logger.LogWarning("Attempted to delete an owner with UUID {OwnerUuid} that was not found.", uuid);
                throw new KeyNotFoundException($"Owner with UUID {uuid} not found.");
            }

            ownerToDelete.Delete();
            await _ownerRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Owner with UUID {OwnerUuid} deleted successfully by IP {RequestingIpAddress}.",
                uuid,
                requestingIpAddress ?? "N/A"
            );
        }

        public async Task<OwnerResponse?> GetOwnerByUuid(Guid uuid)
        {
            var owner = await _ownerRepository.GetOwnerByUuidAsync(uuid);

            if (owner == null)
            {
                _logger.LogWarning("Attempted to get an owner info with UUID {OwnerUuid} that was not found.", uuid);
                throw new KeyNotFoundException($"Owner with UUID {uuid} not found.");
            }

            return owner.MapToResponse();
        }

        public async Task<IEnumerable<OwnerResponseFullInfo>> GetArchivedOwners(string? requestingIpAddress)
        {
            _logger.LogInformation(
                "IP {RequestingIpAddress} is searching for archived owners (deleted ones)",
                requestingIpAddress ?? "N/A"
            );

            var archivedOwners = await _ownerRepository.GetArchivedOwnersAsync();
            var archivedOwnersList = archivedOwners.ToList();

            _logger.LogInformation(
               "Search from IP {RequestingIpAddress} found {OwnersFoundCount} archived owners.",
               requestingIpAddress ?? "N/A",
               archivedOwnersList.Count
           );

            return archivedOwners.MapToEnumerableFullInfoResponse();
        }

        public async Task<OwnerResponse> UpdateOwner(Guid uuid, OwnerRequest dto)
        {
            var owner = await _ownerRepository.GetOwnerByUuidAsync(uuid);

            if (owner == null)
            {
                _logger.LogWarning("Attempted to update an owner info with UUID {OwnerUuid} that was not found.", uuid);
                throw new KeyNotFoundException($"Owner with UUID {uuid} not found.");
            }

            var oldData = new
            {
                Name = owner.Name,
                Address = owner.Address,
                Phone = owner.Phone?.ToString()
            };

            var newData = new
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone
            };

            var phoneNumberObject = string.IsNullOrWhiteSpace(dto.Phone) ? null : PhoneNumber.Create(dto.Phone);

            owner.UpdateName(dto.Name);
            owner.UpdateAddress(dto.Address);
            owner.UpdatePhone(phoneNumberObject?.ToString());

            await _ownerRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Owner {OwnerUuid} updated successfully. Changes {@Changes}",
                uuid,
                new { Old = oldData, New = newData }
            );

            return owner.MapToResponse();
        }

        public async Task<OwnerResponse> ReactivateOwner(Guid uuid, string? requestingIpAddress)
        {
            _logger.LogInformation(
                "IP {RequestingIpAddress} is trying to reactivate an deleted owner with UUID {OwnerUuid}",
                requestingIpAddress ?? "N/A",
                uuid
            );

            var owner = await _ownerRepository.GetOwnerByUuidIgnoringFiltersAsync(uuid);

            if (owner == null)
            {
                _logger.LogWarning("Attempted to reactivate an owner with UUID {OwnerUuid} that was not found.", uuid);
                throw new KeyNotFoundException($"Owner with UUID {uuid} not found.");
            }

            owner.Reactivate();
            await _ownerRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Owner with UUID {OwnerUuid} reactivated by IP {RequestingIpAddress}",
                uuid,
                requestingIpAddress ?? "N/A"
            );

            return owner.MapToResponse();
        }
    }
}
