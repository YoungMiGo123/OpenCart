using OpenCart.Infrastructure.Context;
using OpenCart.Models.Entities;

namespace OpenCart.Repositories.Repositories.CartItemRepository
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(OpenCartDbContext dbContext) : base(dbContext)
        {
        }
    }
}
