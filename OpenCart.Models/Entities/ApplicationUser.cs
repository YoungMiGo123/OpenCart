using OpenCart.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCart.Models.Entities
{
    public class ApplicationUser : Entity
    {
        public string Username { get; init; }
        public string Email { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string AuthProviderId { get; set; }

        public ICollection<CartItem> CartItems { get; init; }
    }
}
