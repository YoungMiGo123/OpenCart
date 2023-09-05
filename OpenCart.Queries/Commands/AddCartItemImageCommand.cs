using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;

namespace OpenCart.Operations.Commands
{
    public class AddCartItemImageCommand
    {
        public string UserId { get; set; }
        public Guid CartItemId { get; set; }
        public CartItemImageDto CartItemImage { get; set; }
    }
}
