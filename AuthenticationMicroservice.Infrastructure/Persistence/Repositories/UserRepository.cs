using AuthenticationMicroservice.Domain.Entities;
using AuthenticationMicroservice.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Persistence.Repositories
{
    internal class UserRepository : IUserRepository
    {
        public Task AddAsync(User user, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
