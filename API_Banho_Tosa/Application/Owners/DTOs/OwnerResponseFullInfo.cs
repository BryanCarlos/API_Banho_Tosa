using Newtonsoft.Json;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public record OwnerResponseFullInfo(
        [JsonProperty("updated_at")]
        DateTime UpdatedAt,

        [JsonProperty("deleted_at")]
        DateTime? DeletedAt,

        Guid Uuid,
        DateTime CreatedAt,
        string Name,
        string? Phone = null,
        string? Address = null)
        : OwnerResponse(Uuid, CreatedAt, Name, Phone, Address
    );
}
