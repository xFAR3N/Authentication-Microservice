using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? CreatedByIp { get; set; }

        public DateTime? RevokedAtUtc { get; set; }

        public string? RevokedByIp { get; set; }

        public string? ReplacedByToken { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

        public bool IsRevoked => RevokedAtUtc != null;

        public bool IsActive => !IsRevoked && !IsExpired;

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
