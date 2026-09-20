using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; init; } = string.Empty;

        public DateTime ExpiresAtUtc { get; init; }

        public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

        public string? CreatedByIp { get; init; }

        public DateTime? RevokedAtUtc { get; private set; }

        public string? RevokedByIp { get; private set; }

        public string? ReplacedByToken { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

        public bool IsRevoked => RevokedAtUtc != null;

        public bool IsActive => !IsRevoked && !IsExpired;

        public Guid UserId { get; private set; }

        public User User { get; set; } = null!;
    }
}
