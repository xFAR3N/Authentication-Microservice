using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.Common.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(string plainPassword);

        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}
