using AuthenticationMicroservice.Application.Common.Interfaces;
using AuthenticationMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Security
{
    internal class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(User user, string plainPassword)
        {
            throw new NotImplementedException();
        }

        public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
        {
            throw new NotImplementedException();
        }
    }
}
