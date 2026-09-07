using Microsoft.AspNetCore.Identity;
using System.Net;
using Todo.Identity.Domain.ValueObjects;

namespace Todo.Identity.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public FullName FullName { get; private set; }
        public Email Email { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        private User(Guid id, FullName fullname, Email email, PhoneNumber? phoneNumber = null)
        {
            Id = id;
            FullName = fullname;
            Email = email;
            PhoneNumber = phoneNumber;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public static User Create(Guid id, FullName fullname, Email email, PhoneNumber? phoneNumber)
        {
            return new User(id, fullname, email, phoneNumber);
        }

        public void UpdateProfile(FullName fullName, PhoneNumber? phoneNumber = null)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
            RevokeRefreshToken();
        }

        public void SetRefreshToken(string refreshToken, DateTime expiryTime)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
        }

        public bool IsRefreshTokenValid(string refreshToken)
        {
            return RefreshToken == refreshToken &&
                   RefreshTokenExpiryTime.HasValue &&
                   RefreshTokenExpiryTime.Value > DateTime.UtcNow;
        }
    }
}
