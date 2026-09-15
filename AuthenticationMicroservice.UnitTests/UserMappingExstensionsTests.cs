using AuthenticationMicroservice.Application.Mappings;
using AuthenticationMicroservice.Domain.Entities;
using FluentAssertions;
using System;
using Xunit;

namespace AuthenticationMicroservice.UnitTests;

public class UserMappingExtensionsTests
{
    [Fact]
    public void ToRegisteredResponse_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "jan.kowalski@example.com",
            UserName = "jankowalski",
            PasswordHash = "some_secret_hash",
            Role = "User",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Act
        var response = user.ToRegisteredResponse();

        // Assert
        response.Should().NotBeNull();
        response.UserId.Should().Be(user.Id);
        response.Email.Should().Be(user.Email);
        response.UserName.Should().Be(user.UserName);
        response.CreatedAtUtc.Should().Be(user.CreatedAtUtc);
    }

    [Fact]
    public void ToProfileResponse_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@example.com",
            UserName = "adminuser",
            PasswordHash = "super_hash",
            Role = "Admin",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Act
        var response = user.ToProfileResponse();

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(user.Id);
        response.Email.Should().Be(user.Email);
        response.Username.Should().Be(user.UserName);
        response.Role.Should().Be("Admin");
        response.CreatedAtUtc.Should().Be(user.CreatedAtUtc);
    }
}