using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Services.DTOs
{
    public record UpdateServicePaymentStatusRequest
    (
        [property: JsonPropertyName("payment_status_id")] 
        int PaymentStatusId
    );
}
