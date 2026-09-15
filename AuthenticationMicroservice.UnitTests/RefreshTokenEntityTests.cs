using AuthenticationMicroservice.Domain.Entities;
using FluentAssertions;
using System;
using Xunit;

namespace AuthenticationMicroservice.UnitTests;

public class RefreshTokenEntityTests
{
    [Fact]
    public void RefreshToken_ShouldBeActive_WhenNotExpiredAndNotRevoked()
    {
        var token = new RefreshToken
        {
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
            RevokedAtUtc = null
        };

        token.IsExpired.Should().BeFalse();
        token.IsRevoked.Should().BeFalse();
        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RefreshToken_ShouldNotBeActive_WhenExpired()
    {
        var token = new RefreshToken
        {
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-1),
            RevokedAtUtc = null
        };

        token.IsExpired.Should().BeTrue();
        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_ShouldNotBeActive_WhenRevoked()
    {
        var token = new RefreshToken
        {
            ExpiresAtUtc = DateTime.UtcNow.AddDays(5),
            RevokedAtUtc = DateTime.UtcNow
        };

        token.IsRevoked.Should().BeTrue();
        token.IsActive.Should().BeFalse();
    }
}