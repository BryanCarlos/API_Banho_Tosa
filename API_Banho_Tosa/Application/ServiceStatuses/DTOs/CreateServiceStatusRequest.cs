using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.ServiceStatuses.DTOs
{
    public record CreateServiceStatusRequest
    (
        [property: JsonPropertyName("description")]  
        string Description
    );
}
