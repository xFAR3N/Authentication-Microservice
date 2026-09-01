using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.Common.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
            : base(message)
        {
            
        }

        public UnauthorizedException()
            : base("Unauthorized access.")
        {

        }
    }
}
