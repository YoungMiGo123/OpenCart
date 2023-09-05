using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCart.Operations.Commands
{
    public class UpdateCartItemCommand
    {
        public string UserId { get; set; }
        public Guid CartItemId { get; set; }
        public CartItemDto CartItem { get; set; }
    }
}
