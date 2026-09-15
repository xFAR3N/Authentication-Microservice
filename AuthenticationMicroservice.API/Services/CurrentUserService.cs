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

                return Guid.TryParse(userIdClaim, out var parsedGuid) ? parsedGuid : Guid.Empty;
            }
        }

        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
