using AuthenticationMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenWithUserAsync(string token, CancellationToken ct = default);

        Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default);

        void Update(RefreshToken refreshToken);
    }
}
