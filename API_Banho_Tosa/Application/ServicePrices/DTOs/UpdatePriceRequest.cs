using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.ServicePrices.DTOs
{
    public record UpdatePriceRequest
    (
        [property: JsonPropertyName("price")]
        decimal Price
    );
}
