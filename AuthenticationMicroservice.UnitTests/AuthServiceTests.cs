using AuthenticationMicroservice.Application.Common.Exceptions;
using AuthenticationMicroservice.Application.Common.Interfaces;
using AuthenticationMicroservice.Application.DTOs.Requests;
using AuthenticationMicroservice.Application.Services;
using AuthenticationMicroservice.Application.Validators;
using AuthenticationMicroservice.Domain.Entities;
using AuthenticationMicroservice.Domain.Exceptions;
using AuthenticationMicroservice.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticationMicroservice.UnitTests;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _tokenRepo = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasherService _passwordHasher = Substitute.For<IPasswordHasherService>();
    private readonly IJwtTokenGenerator _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();

    private readonly RegisterRequestValidator _registerValidator = new();
    private readonly LoginRequestValidator _loginValidator = new();
    private readonly RefreshTokenRequestValidator _refreshTokenValidator = new();

    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(
            _userRepo,
            _tokenRepo,
            _unitOfWork,
            _jwtTokenGenerator,
            _passwordHasher,
            _registerValidator,
            _loginValidator,
            _refreshTokenValidator
        );
    }

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_ShouldThrowValidationException_WhenDataIsInvalid()
    {
        var request = new RegisterRequest("invalid-email", "weak", "");
        var act = () => _sut.RegisterAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowUserAlreadyExistsException_WhenEmailExists()
    {
        var request = new RegisterRequest("taken@example.com", "StrongPass123!", "user1");
        _userRepo.ExistsByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _sut.RegisterAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<UserAlreadyExistsException>()
            .WithMessage("*already taken*");
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowUserAlreadyExistsException_WhenUserNameExists()
    {
        var request = new RegisterRequest("free@example.com", "StrongPass123!", "takenuser");
        _userRepo.ExistsByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(false);
        _userRepo.ExistsByUserNameAsync(request.UserName, Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _sut.RegisterAsync(request, CancellationToken.None);

        await act.Should().ThrowAsync<UserAlreadyExistsException>()
            .WithMessage("*takenuser is already taken*");
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndCommit_WhenDataIsValid()
    {
        var request = new RegisterRequest("valid@example.com", "StrongPass123!", "validuser");
        _userRepo.ExistsByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(false);
        _userRepo.ExistsByUserNameAsync(request.UserName, Arg.Any<CancellationToken>()).Returns(false);
        _passwordHasher.HashPassword(request.Password).Returns("hashed_123");

        var result = await _sut.RegisterAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        result.UserName.Should().Be(request.UserName);

        await _userRepo.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Email == request.Email &&
            u.UserName == request.UserName &&
            u.PasswordHash == "hashed_123" &&
            u.IsActive == true &&
            u.Role == "User"), Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_ShouldThrowInvalidCredentialException_WhenUserNotFound()
    {
        var request = new LoginRequest("nonexistent@example.com", "Password123!");
        _userRepo.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = () => _sut.LoginAsync(request, "127.0.0.1", CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialException>();
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowInvalidCredentialException_WhenUserIsInactive()
    {
        var request = new LoginRequest("blocked@example.com", "Password123!");
        var user = new User { Email = request.Email, IsActive = false };
        _userRepo.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);

        var act = () => _sut.LoginAsync(request, "127.0.0.1", CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialException>();
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowInvalidCredentialException_WhenPasswordDoesNotMatch()
    {
        var request = new LoginRequest("user@example.com", "WrongPassword");
        var user = new User { Email = request.Email, PasswordHash = "hash", IsActive = true };
        _userRepo.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(request.Password, user.PasswordHash).Returns(false);

        var act = () => _sut.LoginAsync(request, "127.0.0.1", CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialException>();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthTokens_WhenCredentialsAreValid()
    {
        var request = new LoginRequest("user@example.com", "CorrectPassword");
        var user = new User { Id = Guid.NewGuid(), Email = request.Email, PasswordHash = "hash", IsActive = true };
        var refreshToken = new RefreshToken { Token = "token_abc", UserId = user.Id };

        _userRepo.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(request.Password, user.PasswordHash).Returns(true);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("jwt_access_token");
        _jwtTokenGenerator.GenerateRefreshToken("127.0.0.1").Returns(refreshToken);

        var result = await _sut.LoginAsync(request, "127.0.0.1", CancellationToken.None);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("jwt_access_token");
        result.RefreshToken.Should().Be("token_abc");

        await _tokenRepo.Received(1).AddAsync(refreshToken, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrowUnauthorizedException_WhenTokenDoesNotExist()
    {
        var request = new RefreshTokenRequest("ghost_token");
        _tokenRepo.GetByTokenWithUserAsync(request.RefreshToken, Arg.Any<CancellationToken>()).Returns((RefreshToken?)null);

        var act = () => _sut.RefreshTokenAsync(request, "127.0.0.1", CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrowUnauthorizedException_WhenTokenIsExpired()
    {
        var request = new RefreshTokenRequest("expired_token");
        var token = new RefreshToken
        {
            Token = "expired_token",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-5),
            User = new User { IsActive = true }
        };
        _tokenRepo.GetByTokenWithUserAsync(request.RefreshToken, Arg.Any<CancellationToken>()).Returns(token);

        var act = () => _sut.RefreshTokenAsync(request, "127.0.0.1", CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrowUnauthorizedException_WhenUserIsBlocked()
    {
        var request = new RefreshTokenRequest("valid_token");
        var token = new RefreshToken
        {
            Token = "valid_token",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
            User = new User { IsActive = false }
        };
        _tokenRepo.GetByTokenWithUserAsync(request.RefreshToken, Arg.Any<CancellationToken>()).Returns(token);

        var act = () => _sut.RefreshTokenAsync(request, "127.0.0.1", CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldRotateTokensSuccessfully_WhenTokenIsValid()
    {
        var request = new RefreshTokenRequest("old_token");
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", IsActive = true };
        var oldToken = new RefreshToken
        {
            Token = "old_token",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
            UserId = user.Id,
            User = user
        };
        var newToken = new RefreshToken { Token = "new_token", UserId = user.Id };

        _tokenRepo.GetByTokenWithUserAsync(request.RefreshToken, Arg.Any<CancellationToken>()).Returns(oldToken);
        _jwtTokenGenerator.GenerateRefreshToken("127.0.0.1").Returns(newToken);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("new_jwt_token");

        var result = await _sut.RefreshTokenAsync(request, "127.0.0.1", CancellationToken.None);

        result.AccessToken.Should().Be("new_jwt_token");
        result.RefreshToken.Should().Be("new_token");

        oldToken.IsRevoked.Should().BeTrue();
        oldToken.ReplacedByToken.Should().Be("new_token");
        oldToken.RevokedByIp.Should().Be("127.0.0.1");

        _tokenRepo.Received(1).Update(oldToken);
        await _tokenRepo.Received(1).AddAsync(newToken, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    #endregion

    #region RevokeTokenAsync & GetCurrentUserProfileAsync Tests

    [Fact]
    public async Task RevokeTokenAsync_ShouldRevokeToken_WhenTokenExistsAndIsActive()
    {
        var request = new RevokeTokenRequest("active_token");
        var token = new RefreshToken
        {
            Token = "active_token",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(2)
        };
        _tokenRepo.GetByTokenAsync(request.RefreshToken, Arg.Any<CancellationToken>()).Returns(token);

        await _sut.RevokeTokenAsync(request, "192.168.1.1", CancellationToken.None);

        token.IsRevoked.Should().BeTrue();
        token.RevokedByIp.Should().Be("192.168.1.1");
        _tokenRepo.Received(1).Update(token);
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RevokeTokenAsync_ShouldDoNothing_WhenTokenNotFound()
    {
        var request = new RevokeTokenRequest("nonexistent");
        _tokenRepo.GetByTokenAsync(request.RefreshToken, Arg.Any<CancellationToken>()).Returns((RefreshToken?)null);

        await _sut.RevokeTokenAsync(request, "127.0.0.1", CancellationToken.None);

        _tokenRepo.DidNotReceive().Update(Arg.Any<RefreshToken>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCurrentUserProfileAsync_ShouldReturnProfile_WhenUserExistsAndActive()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "jan@example.com",
            UserName = "jankowalski",
            Role = "Admin",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.GetCurrentUserProfileAsync(userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be("jan@example.com");
        result.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task GetCurrentUserProfileAsync_ShouldThrowUnauthorizedException_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = () => _sut.GetCurrentUserProfileAsync(userId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    #endregion
}