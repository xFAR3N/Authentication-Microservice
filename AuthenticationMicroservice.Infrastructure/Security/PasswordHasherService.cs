using AuthenticationMicroservice.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure.Security
{
    internal class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(plainPassword, workFactor: 12);
        }

        public bool VerifyPassword(string providedPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(providedPassword, hashedPassword);
        }
    }
}
