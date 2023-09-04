using OpenCart.Models.Entities;
using OpenCart.Repositories.Repositories.GenericRepository;

namespace OpenCart.Repositories.Repositories.CartItemRepository
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        // Additional methods specific to the CartRepository, if any.
    }
}
