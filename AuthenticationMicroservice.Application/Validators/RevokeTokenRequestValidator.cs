using AuthenticationMicroservice.Application.DTOs.Requests;
using FluentValidation;

namespace AuthenticationMicroservice.Application.Validators;

public class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
{
    public RevokeTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}