using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.AvailableServices.DTOs
{
    public record CreateAvailableServiceRequest
    (
        [property: JsonPropertyName("description")]
        string Description,

        [property: JsonPropertyName("duration_in_minutes")]
        int? DurationInMinutes
    );
}
