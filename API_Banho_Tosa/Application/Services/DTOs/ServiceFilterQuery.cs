namespace API_Banho_Tosa.Application.Services.DTOs
{
    public record ServiceFilterQuery
    (
        DateTime? StartDate,
        DateTime? EndDate,
        int? StatusId,
        int? PaymentStatusId,
        Guid? PetId,
        Guid? OwnerId
    );
}
