using AuthenticationMicroservice.Domain.Entities;
using AuthenticationMicroservice.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Persistence.Repositories
{
    internal class RefreshTokenRepository(AuthDbContext context) : IRefreshTokenRepository
    {
        public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
        {
            await context.RefreshTokens.AddAsync(refreshToken, ct);
        }

        public async Task<RefreshToken?> GetByTokenWithUserAsync(string token, CancellationToken ct = default)
        {
            return await context.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token, ct);
        }

        public void Update(RefreshToken refreshToken)
        {
            context.RefreshTokens.Update(refreshToken);
        }
    }
}
