using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System.Net;
using Microsoft.Extensions.Options;
using PetShop.Worker.Email.Services.Configuration;

namespace PetShop.Worker.Email.Services
{
    internal class SmtpEmailSender : IEmailSender
    {
        private readonly ILogger<SmtpEmailSender> _logger;
        private readonly SmtpSettings _smtpSettings;
        private readonly ISmtpClient _smtpClient;
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(ILogger<SmtpEmailSender> logger, IOptions<SmtpSettings> settings, ISmtpClient smtpClient, IConfiguration configuration)
        {
            _logger = logger;
            _smtpSettings = settings.Value;
            _smtpClient = smtpClient;
            _configuration = configuration;
        }

        public async Task SendConfirmationEmailAsync(string email, string confirmationToken, CancellationToken cancellationToken)
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];

            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                throw new InvalidOperationException("The API base URL (ApiSettings:BaseUrl) is not configured.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.DisplayName, _smtpSettings.EmailAddress));
            message.To.Add(new MailboxAddress("PetShop - Registration confirm", email));
            message.Subject = _smtpSettings.Subject;

            var confirmationLink = $"{apiBaseUrl}/api/auth/confirm-email?token={confirmationToken}";

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = $@"
                <!DOCTYPE html>
                <html lang=""pt-br"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Confirmação de Cadastro</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            width: 90%;
                            max-width: 600px;
                            margin: 20px auto;
                            border: 1px solid #ddd;
                            border-radius: 8px;
                            overflow: hidden;
                        }}
                        .header {{
                            background-color: #f7f7f7;
                            padding: 20px;
                            text-align: center;
                        }}
                        .header h1 {{
                            margin: 0;
                            color: #4CAF50;
                        }}
                        .content {{
                            padding: 30px;
                        }}
                        .content p {{
                            margin-bottom: 20px;
                        }}
                        .button-container {{
                            text-align: center;
                            margin: 30px 0;
                        }}
                        .button {{
                            background-color: #4CAF50;
                            color: #ffffff;
                            padding: 12px 25px;
                            text-decoration: none;
                            border-radius: 5px;
                            font-weight: bold;
                            display: inline-block;
                        }}
                        .fallback {{
                            text-align: center;
                            font-size: 12px;
                            color: #888;
                            margin-top: 20px;
                        }}
                        .footer {{
                            background-color: #f7f7f7;
                            padding: 20px;
                            text-align: center;
                            font-size: 12px;
                            color: #888;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h1>Bem-vindo ao PetShop!</h1>
                        </div>
                        <div class=""content"">
                            <p>Olá,</p>
                            <p>Obrigado por se cadastrar! Para ativar sua conta, por favor, clique no botão abaixo. Este link é válido por 1 dia.</p>
            
                            <div class=""button-container"">
                                <a href=""{confirmationLink}"" target=""_blank"" class=""button"">
                                    Ativar Minha Conta
                                </a>
                            </div>
            
                            <p>Atenciosamente,<br>Equipe do Petshop</p>

                            <div class=""fallback"">
                                <p>Se o botão não funcionar, copie e cole o seguinte link no seu navegador:</p>
                                <p><a href=""{confirmationLink}"">{confirmationLink}</a></p>
                            </div>
                        </div>
                        <div class=""footer"">
                            <p>&copy; {DateTime.UtcNow.Year} PetShop Manager. Todos os direitos reservados.</p>
                        </div>
                    </div>
                </body>
                </html>
                "
            };

            try
            {
                await _smtpClient.ConnectAsync(host: _smtpSettings.Host, port: _smtpSettings.Port, options: MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
                await _smtpClient.AuthenticateAsync(_smtpSettings.EmailAddress, _smtpSettings.EmailPassword, cancellationToken);
                await _smtpClient.SendAsync(message, cancellationToken);
                await _smtpClient.DisconnectAsync(quit: true, cancellationToken);

                _logger.LogInformation("Confirmation email sent to {Email} at {SentTime}", email, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an unexpected error trying to send the confirmation email to {Email}", email);
                throw;
            }
        }
    }
}
