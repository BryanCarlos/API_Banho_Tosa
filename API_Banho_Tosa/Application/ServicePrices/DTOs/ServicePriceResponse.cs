using API_Banho_Tosa.Application.AvailableServices.DTOs;
using API_Banho_Tosa.Application.PetSizes.DTOs;
using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.ServicePrices.DTOs
{
    public record ServicePriceResponse
    (
        [property: JsonPropertyName("service_price")]
        decimal ServicePrice,

        [property: JsonPropertyName("pet_size")]
        PetSizeResponse PetSize
    );
}
