using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.AvailableServices.DTOs
{
    public record AvailableServiceResponse
    (
        [property:JsonPropertyName("id")]
        int Id,

        [property:JsonPropertyName("uuid")]
        Guid Uuid,
        
        [property:JsonPropertyName("description")]
        string Description,

        [property:JsonPropertyName("duration_in_minutes")]
        int? DurationInMinutes
    );
}
