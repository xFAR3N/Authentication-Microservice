using AuthenticationMicroservice.Application.DTOs.Requests;
using AuthenticationMicroservice.Application.Validators;
using FluentAssertions;
using Xunit;

namespace AuthenticationMicroservice.UnitTests;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("@missingusername.com")]
    [InlineData("username@.com")]
    public void Should_Fail_When_Email_Is_Invalid(string invalidEmail)
    {
        var model = new RegisterRequest(invalidEmail, "Password123!", "validuser");
        var result = _validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("nocapitol123!")]
    [InlineData("NoDigitSpecial!")]
    [InlineData("NoSpecialDigit123")]
    public void Should_Fail_When_Password_Does_Not_Meet_Complexity(string weakPassword)
    {
        var model = new RegisterRequest("valid@example.com", weakPassword, "validuser");
        var result = _validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Should_Pass_When_All_Fields_Are_Valid()
    {
        var model = new RegisterRequest("valid@example.com", "CorrectPassword123!", "validuser");
        var result = _validator.Validate(model);

        result.IsValid.Should().BeTrue();
    }
}