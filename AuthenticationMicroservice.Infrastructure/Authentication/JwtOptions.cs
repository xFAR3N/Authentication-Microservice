using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Authentication
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public string SecretKey { get; set;  } = string.Empty;

        public int AccessTokenExpirationMinutes { get; set; } = 15;

        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
