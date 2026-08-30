using AuthenticationMicroservice.Domain.Entities;
using AuthenticationMicroservice.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Persistence.Repositories
{
    internal class UserRepository(AuthDbContext context) : IUserRepository
    {
        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await context.Users.AddAsync(user, ct);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        {
            return await context.Users.AsNoTracking().AnyAsync(u => u.Email == email, ct);
        }

        public async Task<bool> ExistsByUserNameAsync(string username, CancellationToken ct = default)
        {
            return await context.Users.AsNoTracking().AnyAsync(u => u.UserName == username, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public void Update(User user)
        {
            context.Users.Update(user);
        }
    }
}
