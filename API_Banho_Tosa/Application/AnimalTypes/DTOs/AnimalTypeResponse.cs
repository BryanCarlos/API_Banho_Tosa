using Newtonsoft.Json;

namespace API_Banho_Tosa.Application.AnimalTypes.DTOs
{
    public record AnimalTypeResponse(
        [JsonProperty("id")]
        int Id,

        [JsonProperty("name")]
        string Name
    );
}
