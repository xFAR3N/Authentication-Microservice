using AuthenticationMicroservice.Application.Common.Exceptions;
using AuthenticationMicroservice.Application.Common.Interfaces;
using System.Security.Claims;

namespace AuthenticationMicroservice.API.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public Guid UserId
        {
            get
            {
                var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var parsedGuid))
                {
                    throw new UnauthorizedException("User identifier claim is missing or invalid.");
                }

                return parsedGuid;
            }
        }

        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
