using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime CreateAtUtc { get; set; } = DateTime.UtcNow;

        public string? CreateByIp { get; set; }

        public DateTime? RevokedAtUc { get; set; }

        public string? RevokedByIp { get; set; }

        public string? ReplacedByToken { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

        public bool IsRevoked => RevokedAtUc != null;

        public bool IsActive => !IsRevoked && !IsExpired;

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
