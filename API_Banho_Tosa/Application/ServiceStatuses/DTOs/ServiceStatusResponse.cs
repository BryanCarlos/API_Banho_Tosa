using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.ServiceStatuses.DTOs
{
    public record ServiceStatusResponse
    (
        [property: JsonPropertyName("id")]
        int Id,

        [property: JsonPropertyName("description")]
        string Description
    );
}
