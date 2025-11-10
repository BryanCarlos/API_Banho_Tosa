using API_Banho_Tosa.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Banho_Tosa.Domain.Entities
{
    [Table("users")]
    public class User
    {
        [Column("user_uuid")]
        public Guid Id { get; private set; }

        [Column("user_email")]
        public Email Email { get; private set; }

        [Column("user_name")]
        [MaxLength(100)]
        public string Username { get; private set; }

        [Column("user_password_hash")]
        public string PasswordHash { get; private set; }

        [Column("user_email_confirmed")]
        public bool IsEmailConfirmed { get; private set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; private set; }

        [Column("user_refresh_token")]
        public string? RefreshToken { get; private set; }

        [Column("user_refresh_token_expiry_time")]
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        [Column("user_email_confirmation_token")]
        public string? EmailConfirmationToken { get; private set; }

        [Column("user_email_token_expiry_date")]
        public DateTime? EmailTokenExpiryDate { get; private set; }

        [Column("user_last_login")]
        public DateTime? LastLogin { get; private set; }

        public ICollection<Role> Roles { get; } = new List<Role>();

        private User() 
        {
            this.Email = null!;
            this.Username = string.Empty;
            this.PasswordHash = string.Empty;
        }

        private User(Email email, string username, string passwordHash)
        {
            var now = DateTime.UtcNow;
            this.Id = Guid.CreateVersion7();
            this.Email = email;
            this.Username = username;
            this.PasswordHash = passwordHash;
            this.IsEmailConfirmed = false;
            this.EmailConfirmationToken = Guid.CreateVersion7().ToString();
            this.EmailTokenExpiryDate = now.AddDays(1);

            this.CreatedAt = now;
            this.UpdatedAt = now;
        }

        public static User Create(Email email, string username, string passwordHash)
        {
            ValidateUsername(username);

            ValidatePassword(passwordHash);

            return new User(email, username, passwordHash);
        }

        public void UpdateUsername(string username)
        {
            ValidateUsername(username);

            this.Username = username;
            MarkAsUpdated();
        }

        public void UpdatePassword(string passwordHash)
        {
            ValidatePassword(passwordHash);

            this.PasswordHash = passwordHash;
            MarkAsUpdated();
        }

        public void UpdateRefreshToken(string refreshToken)
        {
            this.RefreshToken = refreshToken;
            this.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            MarkAsUpdated();
        }

        public void Delete()
        {
            if (DeletedAt.HasValue)
            {
                throw new InvalidOperationException("The user has already been deleted.");
            }

            this.DeletedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        private static void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("The user name cannot be null or empty.");
            }

            if (username.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(Username), "The user name cannot exceed 100 characters.");
            }
        }

        private static void ValidatePassword(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("The user password cannot be null or empty.");
            }
        }

        private void MarkAsUpdated()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void CreateEmailToken()
        {
            this.EmailConfirmationToken = Guid.CreateVersion7().ToString();
            this.EmailTokenExpiryDate = DateTime.UtcNow.AddDays(1);
            MarkAsUpdated();
        }

        public void ClearRefreshToken()
        {
            this.RefreshToken = null;
            this.RefreshTokenExpiryTime = null;
            MarkAsUpdated();
        }

        public bool IsRefreshTokenValid()
        {
            return this.RefreshToken != null && this.RefreshTokenExpiryTime >= DateTime.UtcNow;
        }

        public void UpdateLastLogin()
        {
            this.LastLogin = DateTime.UtcNow;
        }

        public void ConfirmEmail()
        {
            this.IsEmailConfirmed = true;
            this.EmailConfirmationToken = null;
            this.EmailTokenExpiryDate = null;
            MarkAsUpdated();
        }
    }
}
