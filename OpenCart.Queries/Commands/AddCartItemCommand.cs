using OpenCart.Models.Entities;

namespace OpenCart.Operations.Commands
{
    public class AddCartItemCommand
    {
        public string UserId { get; set; }
        public CartItem CartItem { get; set; }
    }
}
