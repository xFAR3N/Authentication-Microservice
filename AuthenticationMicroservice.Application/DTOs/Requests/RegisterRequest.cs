using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.DTOs.Requests
{
    public record RegisterRequest(string Email, string Password, string UserName);
}
