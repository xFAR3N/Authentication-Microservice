using AuthenticationMicroservice.Application.Common.Interfaces;
using AuthenticationMicroservice.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Authentication
{
    internal class JwtTokenGenerator(IOptions<JwtOptions> jwtOptions) : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Role),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public RefreshToken GenerateRefreshToken(string? ipAddress)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            var tokenString = Convert.ToBase64String(randomBytes);

            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = tokenString,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByIp = ipAddress,
            };
        }

        int AccesstokenExpirationSeconds => (int)TimeSpan.FromMinutes(_jwtOptions.AccessTokenExpirationMinutes).TotalSeconds;
    }
}
