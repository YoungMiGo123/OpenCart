using OpenCart.Infrastructure.Context;
using OpenCart.Models.Entities;
using OpenCart.Repositories.Repositories.CartItemImageRepository;

namespace OpenCart.Repositories.Repositories.CartItemRepository
{
    public class CartItemImageRepository : GenericRepository<CartItemImage>, ICartItemImageRepository
    {
        public CartItemImageRepository(OpenCartDbContext dbContext) : base(dbContext)
        {
        }
    }
}
