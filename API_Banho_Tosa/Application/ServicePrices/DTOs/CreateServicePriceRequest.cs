using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.ServicePrices.DTOs
{
    public record CreateServicePriceRequest
    (
        [property: JsonPropertyName("available_service_id")]
        int AvailableServiceId,

        [property: JsonPropertyName("pet_size_id")]
        int PetSizeId,

        [property: JsonPropertyName("service_price")]
        decimal ServicePrice
    );
}
