using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.DTOs.Requests
{
    public record RevokeTokenRequest(string RefreshToken);
}
