using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Domain.Exceptions
{
    public class InvalidCredentialException : DomainException
    {
        public InvalidCredentialException(string message = "Invalid Email and/or Password") : base(message)
        {
        }
    }
}
