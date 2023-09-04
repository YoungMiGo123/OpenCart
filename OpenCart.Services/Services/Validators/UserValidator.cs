using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCart.Services.Services.Validators
{
    public class UserValidator : AbstractValidator<Application>
    {
        public UserValidator()
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Username is required.")
                .EmailAddress().WithMessage("Username must be a valid email address.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(20).WithMessage("First name must not exceed 20 characters.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(20).WithMessage("Last name must not exceed 20 characters.");
        }
    }
    
    
}
