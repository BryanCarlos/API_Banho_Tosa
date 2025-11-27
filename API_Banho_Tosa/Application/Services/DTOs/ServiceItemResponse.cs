using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Services.DTOs
{
    public record ServiceItemResponse
    (
        [property: JsonPropertyName("id")]
        int Id,

        [property: JsonPropertyName("name")]
        string ServiceName,

        [property: JsonPropertyName("unit_price")]
        decimal UnitPrice,

        [property: JsonPropertyName("quantity")]
        int Quantity,

        [property: JsonPropertyName("total_item")]
        decimal TotalItem
    );
}
