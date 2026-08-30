using AuthenticationMicroservice.Application.DTOs.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(lr => lr.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("Invalid email address format.")
                .MaximumLength(256).WithMessage("Email address cannot be longer than 256 characters.");

            RuleFor(lr => lr.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
