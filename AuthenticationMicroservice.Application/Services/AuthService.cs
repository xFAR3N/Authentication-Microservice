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

namespace AuthenticationMicroservice.Application.Services
{
    internal class AuthService(IUserRepository _userRepo, IRefreshTokenRepository _tokeRepo, IUnitOfWork _unitOfWork, IJwtTokenGenerator _tokeGenerator, IPasswordHasherService _passwordHasher, IValidator<RegisterRequest> _registerValidator, IValidator<LoginRequest> _loginValidator, IValidator<RefreshTokenRequest> _refreshTokenValidator) : IAuthService
    {
        public async Task<UserProfileResponse> GetCurrentUserProfileAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepo.GetByIdAsync(userId, ct);

            if(user == null || !user.IsActive)
            {
                throw new UnauthorizedException("User does not exist or session is no longer valid");
            }

            return user.ToProfileResponse();
        }

        public async Task<AuthTokenResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct = default)
        {
            var validationResult = await _loginValidator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors.ToString() ?? "Error data validation.");
            }

            var user = await _userRepo.GetByEmailAsync(request.Email, ct);

            if (user == null || !user.IsActive)
            {
                throw new InvalidCredentialException("Nieprawidłowy adres email lub hasło.");
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new InvalidCredentialException("Nieprawidłowy adres email lub hasło.");
            }

            var accessToken = _tokeGenerator.GenerateAccessToken(user);

            var refreshToken = _tokeGenerator.GenerateRefreshToken(ipAddress);

            refreshToken.UserId = user.Id;

            await _tokeRepo.AddAsync(refreshToken, ct);

            await _unitOfWork.CommitAsync(ct);

            return new AuthTokenResponse(accessToken, 900, refreshToken.Token);
        }

        public async Task<AuthTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken ct = default)
        {
            var validationResult = await _refreshTokenValidator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors.ToString() ?? "Error data validation.");
            }

            var existingToken = await _tokeRepo.GetByTokenWithUserAsync(request.RefreshToken, ct);

            if (existingToken == null || !existingToken.IsActive || !existingToken.User.IsActive)
            {
                throw new UnauthorizedException("Nieprawidłowy lub wygasły token odświeżający.");
            }

            var newRefreshToken = _tokeGenerator.GenerateRefreshToken(ipAddress);

            newRefreshToken.UserId = existingToken.UserId;

            existingToken.RevokedAtUtc = DateTime.UtcNow;

            existingToken.RevokedByIp = ipAddress;

            existingToken.ReplacedByToken = newRefreshToken.Token;

            _tokeRepo.Update(existingToken);

            await _tokeRepo.AddAsync(newRefreshToken, ct);

            var newAccessToken = _tokeGenerator.GenerateAccessToken(existingToken.User);

            await _unitOfWork.CommitAsync(ct);

            return new AuthTokenResponse(newAccessToken, 900, newRefreshToken.Token);
        }

        public async Task<UserRegisteredResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var validationResult = await _registerValidator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors.ToString() ?? "Error data validation.");
            }

            if(await _userRepo.ExistsByEmailAsync(request.Email, ct))
            {
                throw new UserAlreadyExistsException($"Email: {request.Email} already taken");
            }

            if(await _userRepo.ExistsByUserNameAsync(request.UserName, ct))
            {
                throw new UserAlreadyExistsException($"{request.UserName} is already taken");
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);

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

            await _userRepo.AddAsync(user, ct);

            await _unitOfWork.CommitAsync(ct);

            return user.ToRegisteredResponse();
        }

        public async Task RevokeTokenAsync(RevokeTokenRequest request, string? ipAddress, CancellationToken ct = default)
        {
            var token = await _tokeRepo.GetByTokenAsync(request.RefreshToken, ct);

            if(token != null && token.IsActive)
            {
                token.RevokedAtUtc = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;

                _tokeRepo.Update(token);

                await _unitOfWork.CommitAsync(ct);
            }
        }
    }
}
