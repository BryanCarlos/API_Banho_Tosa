using System.ComponentModel;

namespace API_Banho_Tosa.Domain.Enums
{
    public enum ServiceStatusEnum
    {
        [Description("Serviço agendado")]
        Agendado = 1,

        [Description("Serviço concluido")]
        Concluido = 2,

        [Description("Serviço cancelado")]
        Cancelado = 3
    }
}
