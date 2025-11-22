using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.PaymentStatuses.DTOs
{
    public record CreatePaymentStatusRequest
    (
        [property: JsonPropertyName("description")]
        string Description  
    );
}
