using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Services.DTOs
{
    public record CreateServiceRequest
    (
        [property: JsonPropertyName("pet_id")]
        Guid PetId,

        [property: JsonPropertyName("service_date")]
        DateTime ServiceDate,

        [property: JsonPropertyName("service_items")]
        IEnumerable<int> AvailableServicesId,

        [property: JsonPropertyName("discount_value")]
        decimal? DiscountValue = 0,

        [property: JsonPropertyName("additional_charges")]
        decimal? AdditionalCharges = 0
    );
}
