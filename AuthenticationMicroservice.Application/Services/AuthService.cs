using AuthenticationMicroservice.Application.Common.Exceptions;
using AuthenticationMicroservice.Application.Common.Interfaces;
using AuthenticationMicroservice.Application.DTOs.Requests;
using AuthenticationMicroservice.Application.DTOs.Responses;
using AuthenticationMicroservice.Application.Mappings;
using AuthenticationMicroservice.Domain.Entities;
using AuthenticationMicroservice.Domain.Exceptions;
using AuthenticationMicroservice.Domain.Interfaces;
using FluentValidation;
using ValidationException = AuthenticationMicroservice.Application.Common.Exceptions.ValidationException;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace AuthenticationMicroservice.Application.Services
{
    internal class AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository tokenRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator, 
        IPasswordHasherService passwordHasherService, 
        IValidator<RegisterRequest> registerRequestValidator, 
        IValidator<LoginRequest> loginRequestValidator, 
        IValidator<RefreshTokenRequest> refreshTokenRequestValidator,
        ILogger<AuthService> logger) : IAuthService
    {
        public async Task<UserProfileResponse> GetCurrentUserProfileAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await userRepository.GetByIdAsync(userId, ct);

            if(user == null || !user.IsActive)
            {
                throw new UnauthorizedException("User does not exist or session is no longer valid");
            }

            return user.ToProfileResponse();
        }

        public async Task<AuthTokenResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct = default)
        {
            var validationResult = await loginRequestValidator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                logger.LogWarning("Failed login attempt for email {Email} from IP {IpAddress}.", request.Email, ipAddress);
                throw new ValidationException(errors);
            }

            var user = await userRepository.GetByEmailAsync(request.Email, ct);

            if (user == null || !user.IsActive)
            {
                logger.LogWarning("Failed login attempt for email {Email} from IP {IpAddress}.", request.Email, ipAddress);
                throw new InvalidCredentialException("Invalid email or password.");
            }

            if (!passwordHasherService.VerifyPassword(request.Password, user.PasswordHash))
            {
                logger.LogWarning("Failed login attempt for email {Email} from IP {IpAddress}.", request.Email, ipAddress);
                throw new InvalidCredentialException("Invalid email or password.");
            }

            var accessToken = jwtTokenGenerator.GenerateAccessToken(user);

            var expiresIn = jwtTokenGenerator.AccessTokenExpirationSeconds;

            var refreshToken = jwtTokenGenerator.GenerateRefreshToken(ipAddress);

            refreshToken.UserId = user.Id;

            await tokenRepository.AddAsync(refreshToken, ct);

            await unitOfWork.CommitAsync(ct);

            logger.LogInformation("User {UserId} logged in successfully from IP {IpAddress}.", user.Id, ipAddress);

            return new AuthTokenResponse(accessToken, expiresIn, refreshToken.Token);
        }

        public async Task<AuthTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken ct = default)
        {
            var validationResult = await refreshTokenRequestValidator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                logger.LogWarning("Potential token reuse or invalid token attempt from IP {IpAddress}.", ipAddress);
                throw new ValidationException(errors);
            }

            var existingToken = await tokenRepository.GetByTokenWithUserAsync(request.RefreshToken, ct);

            if (existingToken == null || !existingToken.IsActive || !existingToken.User.IsActive)
            {
                logger.LogWarning("Potential token reuse or invalid token attempt from IP {IpAddress}.", ipAddress);
                throw new UnauthorizedException("Invalid or expired refresh token.");
            }

            var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken(ipAddress);

            newRefreshToken.UserId = existingToken.UserId;

            existingToken.RevokedAtUtc = DateTime.UtcNow;

            existingToken.RevokedByIp = ipAddress;

            existingToken.ReplacedByToken = newRefreshToken.Token;

            tokenRepository.Update(existingToken);

            await tokenRepository.AddAsync(newRefreshToken, ct);

            var newAccessToken = jwtTokenGenerator.GenerateAccessToken(existingToken.User);

            await unitOfWork.CommitAsync(ct);

            logger.LogInformation("Refresh token rotated successfully for user {UserId} from IP {IpAddress}.", newRefreshToken.UserId, ipAddress);

            return new AuthTokenResponse(newAccessToken, 900, newRefreshToken.Token);
        }

        public async Task<UserRegisteredResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var validationResult = await registerRequestValidator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }

            if(await userRepository.ExistsByEmailAsync(request.Email, ct))
            {
                throw new UserAlreadyExistsException($"Email: {request.Email} already taken");
            }

            if(await userRepository.ExistsByUserNameAsync(request.UserName, ct))
            {
                throw new UserAlreadyExistsException($"{request.UserName} is already taken");
            }

            var passwordHash = passwordHasherService.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.UserName,
                PasswordHash = passwordHash,
                Role = "User",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
            };

            await userRepository.AddAsync(user, ct);

            await unitOfWork.CommitAsync(ct);

            logger.LogInformation("User {UserId} successfully registered with email {Email}.", user.Id, user.Email);

            return user.ToRegisteredResponse();
        }

        public async Task RevokeTokenAsync(RevokeTokenRequest request, string? ipAddress, CancellationToken ct = default)
        {
            var token = await tokenRepository.GetByTokenAsync(request.RefreshToken, ct);

            if(token != null && token.IsActive)
            {
                token.RevokedAtUtc = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;

                tokenRepository.Update(token);

                logger.LogInformation("Refresh token revoked from IP {IpAddress}.", ipAddress);

                await unitOfWork.CommitAsync(ct);
            }
        }
    }
}
