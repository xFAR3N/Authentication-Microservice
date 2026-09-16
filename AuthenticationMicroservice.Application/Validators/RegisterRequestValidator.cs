using AuthenticationMicroservice.Application.DTOs.Requests;
using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(rr => rr.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("Invalid email address format.")
                .Matches(@"^[^@\s]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")
                    .WithMessage("Invalid email address format.")
                .MaximumLength(256).WithMessage("Email address cannot be longer than 256 characters.");

            RuleFor(rr => rr.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .MaximumLength(128).WithMessage("Password cannot exceed 128 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one capitol letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain special character");

            RuleFor(rr => rr.UserName)
                .NotEmpty().WithMessage("Username cannot be empty")
                .MaximumLength(100).WithMessage("Username cannot be longer than 100 characters");
        }
    }
}
