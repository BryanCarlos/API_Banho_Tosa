using API_Banho_Tosa.Application.PaymentStatuses.DTOs;
using API_Banho_Tosa.Application.Pets.DTOs;
using API_Banho_Tosa.Application.ServiceStatuses.DTOs;
using System.Text.Json.Serialization;

namespace API_Banho_Tosa.Application.Services.DTOs
{
    public record ServiceResponse
    (
        [property: JsonPropertyName("id")]
        Guid Id,

        [property: JsonPropertyName("service_date")]
        DateTime ServiceDate,

        [property: JsonPropertyName("status")]
        ServiceStatusResponse Status,

        [property: JsonPropertyName("payment_status")]
        PaymentStatusResponse PaymentStatus,

        [property: JsonPropertyName("payment_date")]
        DateTime? PaymentDate,

        [property: JsonPropertyName("payment_due_date")]
        DateTime? PaymentDueTime,

        [property: JsonPropertyName("subtotal")]
        decimal Subtotal,

        [property: JsonPropertyName("discount_value")]
        decimal DiscountValue,

        [property: JsonPropertyName("additional_charges")]
        decimal AdditionalCharges,

        [property: JsonPropertyName("total")]
        decimal Total,

        [property: JsonPropertyName("pet")]
        PetResponse Pet,

        [property: JsonPropertyName("items")]
        IEnumerable<ServiceItemResponse> Items
    );
}
