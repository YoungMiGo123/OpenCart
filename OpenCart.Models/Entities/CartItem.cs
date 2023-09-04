using OpenCart.Common;
using System.Collections.ObjectModel;

namespace OpenCart.Models.Entities
{
    public class CartItem : Entity
    {
        public CartItem()
        {
            CartItemImages = new Collection<CartItemImage>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<CartItemImage> CartItemImages { get; set; }
    }
}
