using AuthenticationMicroservice.Application.DTOs.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.Validators
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(rt => rt.RefreshToken)
                .NotEmpty().WithMessage("RefreshToken cannot be empty.");
        }
    }
}
