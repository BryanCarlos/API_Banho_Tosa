namespace PetShop.Worker.Email.Services
{
    public interface IEmailSender
    {
        Task SendConfirmationEmailAsync(string email, string confirmationToken, CancellationToken cancellationToken);
    }
}
