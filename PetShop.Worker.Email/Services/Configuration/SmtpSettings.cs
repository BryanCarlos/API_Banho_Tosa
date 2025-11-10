namespace PetShop.Worker.Email.Services.Configuration
{
    internal record SmtpSettings
    {
        public string DisplayName { get; init; } = string.Empty;
        public string EmailAddress { get; init; } = string.Empty;
        public string EmailPassword { get; init; } = string.Empty;
        public string Host { get; init; } = string.Empty;
        public int Port { get; init; } = 587;
        public string Subject { get; init; } = string.Empty;
    }
}
