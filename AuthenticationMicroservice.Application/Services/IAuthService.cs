using AuthenticationMicroservice.Application.DTOs.Requests;
using AuthenticationMicroservice.Application.DTOs.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationMicroservice.Application.Services
{
    public interface IAuthService
    {
        Task<UserRegisteredResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

        Task<AuthTokenResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct = default);

        Task<AuthTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken ct = default);

        Task RevokeTokenAsync(RevokeTokenRequest request, string? ipAddress, CancellationToken ct = default);

        Task<UserProfileResponse> GetCurrentUserProfileAsync(Guid userId, CancellationToken ct = default);
    }
}
