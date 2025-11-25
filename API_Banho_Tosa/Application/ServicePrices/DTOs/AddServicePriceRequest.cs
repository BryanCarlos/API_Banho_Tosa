using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.ServicePrices.DTOs
{
    public record AddServicePriceRequest(
        [property: JsonPropertyName("pet_size_id")]
        int PetSizeId,

        [property: JsonPropertyName("price")]
        decimal Price
    );
}
