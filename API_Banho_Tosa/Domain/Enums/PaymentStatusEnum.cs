using System.ComponentModel;

namespace API_Banho_Tosa.Domain.Enums
{
    public enum PaymentStatusEnum
    {
        [Description("Aguardando Pagamento")]
        Pendente = 1,

        [Description("Pagamento Confirmado")]
        Pago = 2,

        [Description("Cancelado pelo Usuário")]
        Cancelado = 3
    }
}
