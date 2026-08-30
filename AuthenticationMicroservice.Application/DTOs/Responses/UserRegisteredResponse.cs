using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.DTOs.Responses
{
    public record UserRegisteredResponse(Guid UserId, string Email, string UserName, DateTime CreatedAtUtc);
}
