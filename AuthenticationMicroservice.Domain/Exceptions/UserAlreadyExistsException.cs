using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Domain.Exceptions
{
    public class UserAlreadyExistsException : DomainException
    {
        public UserAlreadyExistsException(string message = "User already exists") : base(message)
        {
        }
    }
}
