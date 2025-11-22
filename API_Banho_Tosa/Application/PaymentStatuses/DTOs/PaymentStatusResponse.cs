using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.PaymentStatuses.DTOs
{
    public record PaymentStatusResponse
    (
        [property: JsonPropertyName("id")]
        int Id,
        
        [property: JsonPropertyName("description")]
        string Description
    );
}
