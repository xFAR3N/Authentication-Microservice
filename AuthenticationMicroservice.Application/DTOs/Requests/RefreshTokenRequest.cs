using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.DTOs.Requests
{
    public record RefreshTokenRequest(string RefreshToken);
}
