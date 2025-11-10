using MailKit.Net.Smtp;
using PetShop.Worker.Email.EventHandlers;
using PetShop.Worker.Email.Services;
using PetShop.Worker.Email.Services.Configuration;
using RabbitMQ.Client;

namespace PetShop.Worker.Email
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

                    var factory = new ConnectionFactory 
                    { 
                        HostName = configuration["RabbitMQ:HostName"]!,
                        UserName = configuration["RabbitMQ:Username"]!,
                        Password = configuration["RabbitMQ:Password"]!,
                    };
                    var rabbitConnection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

                    services.AddSingleton<IEmailSender, SmtpEmailSender>();
                    services.AddSingleton<ISmtpClient, SmtpClient>();
                    services.AddScoped<UserRegisteredEventHandler>();

                    services.AddSingleton(rabbitConnection);

                    services.AddHostedService<EmailWorker>();
                });

            var host = builder.Build();
            host.Run();
        }
    }
}