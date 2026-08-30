using AuthenticationMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

        Task<bool> ExistsByUserNameAsync(string username, CancellationToken ct = default);

        Task AddAsync(User user, CancellationToken ct = default);

        void Update(User user);
    }
}
