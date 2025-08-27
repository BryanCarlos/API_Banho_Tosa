using Newtonsoft.Json;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public record OwnerResponseFullInfo(
        Guid Uuid,
        string Name,
        string? Phone,
        string? Address,
        DateTime CreatedAt,

        [JsonProperty("updated_at")]
        DateTime UpdatedAt,

        [JsonProperty("deleted_at")]
        DateTime? DeletedAt) : OwnerResponse(Uuid, Name, Phone, Address, CreatedAt
    );
}
