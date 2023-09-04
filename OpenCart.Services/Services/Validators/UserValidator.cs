using FluentValidation;
using OpenCart.Models.Entities;

namespace OpenCart.Services.Services.Validators
{
    public class UserValidator : AbstractValidator<ApplicationUser>
    {
        public UserValidator()
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Username is required. Ensure you add a valid username")
                .EmailAddress().WithMessage("Username must be a valid email address.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required. Ensure you add a valid email")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required. Ensure you add a valid firstname")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required. Ensure you add a valid lastname")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
        }
    }


}
