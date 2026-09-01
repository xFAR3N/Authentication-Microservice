using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {
            
        }

        public ValidationException()
            : base("Validation error")
        {
            
        }
    }
}
