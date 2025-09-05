using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Owners.DTOs
{
    public record OwnerResponseFullInfo(
        [property: JsonPropertyName("updated_at")]
        DateTime UpdatedAt,

        [property: JsonPropertyName("deleted_at")]
        DateTime? DeletedAt,

        Guid Uuid,
        DateTime CreatedAt,
        string Name,
        string? Phone = null,
        string? Address = null)
        : OwnerResponse(Uuid, CreatedAt, Name, Phone, Address
    );
}
