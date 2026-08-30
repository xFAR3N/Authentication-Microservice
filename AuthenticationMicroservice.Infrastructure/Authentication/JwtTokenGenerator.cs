using AuthenticationMicroservice.Application.Common.Interfaces;
using AuthenticationMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Authentication
{
    internal class JwtTokenGenerator : IJwtTokenGenerator
    {
        public string GenerateAccessToken(User user)
        {
            throw new NotImplementedException();
        }

        public RefreshToken GenerateRefreshToken(string? ipAddress)
        {
            throw new NotImplementedException();
        }
    }
}
