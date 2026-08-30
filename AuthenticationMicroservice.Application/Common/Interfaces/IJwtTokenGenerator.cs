using AuthenticationMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);

        RefreshToken GenerateRefreshToken(string? ipAddress);
    }
}
