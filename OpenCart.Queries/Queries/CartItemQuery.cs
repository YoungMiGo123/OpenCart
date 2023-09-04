using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCart.Operations.Queries
{
    public class CartItemQuery
    {
        public string UserId { get; set; }
        public Guid CartItemId { get; set; }
    }
}
