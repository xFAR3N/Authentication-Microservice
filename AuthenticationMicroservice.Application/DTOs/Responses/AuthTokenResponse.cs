using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.DTOs.Responses
{
    public record AuthTokenResponse(string AccessToken, int ExpiresIn, string RefreshToken, string TokenType = "Bearer");
}
