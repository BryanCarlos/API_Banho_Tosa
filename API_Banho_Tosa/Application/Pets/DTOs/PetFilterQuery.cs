namespace API_Banho_Tosa.Application.Pets.DTOs
{
    public record PetFilterQuery
    (
        string? PetName, 
        
        string? OwnerName, 
        
        Guid? OwnerId
    );
}
