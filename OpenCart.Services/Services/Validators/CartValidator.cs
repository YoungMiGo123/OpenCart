using FluentValidation;
using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;

namespace OpenCart.Services.Services.Validators
{
    public class CartValidator : AbstractValidator<CartItemDto>
    {
        public CartValidator()
        {
            RuleFor(item => item.Description).MaximumLength(500);
            RuleFor(item => item.Price).GreaterThan(0);
            RuleFor(item => item.Name).NotEmpty().MaximumLength(155);
            RuleFor(item => item.Quantity).GreaterThan(0);
        }
    }
}
