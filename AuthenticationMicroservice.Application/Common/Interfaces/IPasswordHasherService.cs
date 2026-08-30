using AuthenticationMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.Common.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string plainPassword);

        bool VerifyPassword(User user, string hashedPassword, string providedPassword);
    }
}
