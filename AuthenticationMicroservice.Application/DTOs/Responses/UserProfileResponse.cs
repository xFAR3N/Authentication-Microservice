using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.DTOs.Responses
{
    public record UserProfileResponse(Guid Id, string Email, string Username, string Role, DateTime CreatedAtUtc);
}
