using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Services.DTOs
{
    public record UpdateServiceServiceStatusRequest
    (
        [property: JsonPropertyName("status_id")] 
        int StatusId
    );
}
