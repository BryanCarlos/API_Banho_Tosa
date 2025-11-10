using PetShop.Shared.Contracts.Events;
using PetShop.Worker.Email.Services;
using System.Net.Mail;
using System.Threading.Channels;

namespace PetShop.Worker.Email.EventHandlers
{
    public class UserRegisteredEventHandler
    {
        private readonly ILogger<UserRegisteredEventHandler> _logger;
        private readonly IEmailSender _emailSender;

        public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task HandleAsync(UserRegisteredEvent? message, CancellationToken cancellationToken)
        {
            if (message is null)
            {
                throw new ArgumentException("Email message cannot be empty");
            }

            if (message.ExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogInformation("The confirmation email to {Email} expired at {ExpiredAt}", message.Email, DateTime.UtcNow);
                return;
            }

            await _emailSender.SendConfirmationEmailAsync(message.Email, message.Token, cancellationToken);
        }
    }
}
