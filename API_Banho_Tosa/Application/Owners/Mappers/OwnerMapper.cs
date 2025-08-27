using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Domain.Entities;

namespace API_Banho_Tosa.Application.Owners.Mappers
{
    public static class OwnerMapper
    {
        public static IEnumerable<OwnerResponse> MapToEnumerableResponse(this IEnumerable<Owner> owners)
        {
            return owners.Select(MapToResponse);
        }

        public static OwnerResponse MapToResponse(this Owner owner)
        {
            return new OwnerResponse
            (
                Uuid: owner.Uuid,
                Name: owner.Name,
                Address: owner.Address,
                Phone: owner.Phone?.Value,
                CreatedAt: owner.CreatedAt
            );
        }

        public static IEnumerable<OwnerResponseFullInfo> MapToEnumerableFullInfoResponse(this IEnumerable<Owner> owners)
        {
            return owners.Select(MapToFullInfoResponse);
        }

        public static OwnerResponseFullInfo MapToFullInfoResponse(this Owner owner)
        {
            return new OwnerResponseFullInfo
            (
                Uuid: owner.Uuid,
                Name: owner.Name,
                Address: owner.Address,
                Phone: owner.Phone?.Value,
                CreatedAt: owner.CreatedAt,
                UpdatedAt: owner.UpdatedAt,
                DeletedAt: owner.DeletedAt
            );
        }
    }
}
