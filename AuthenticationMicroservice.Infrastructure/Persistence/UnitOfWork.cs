using AuthenticationMicroservice.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Persistence
{
    internal class UnitOfWork(AuthDbContext context) : IUnitOfWork
    {
        public async Task<int> CommitAsync(CancellationToken ct = default)
        {
            return await context.SaveChangesAsync(ct);
        }
    }
}
