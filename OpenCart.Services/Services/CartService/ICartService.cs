using OpenCart.Common;
using OpenCart.Models.Entities;
using OpenCart.Operations.Commands;
using OpenCart.Operations.Queries;

namespace OpenCart.Services.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResult<PaginationResponse<CartItem>>> GetCartItemsAsync(string userId);

        Task<ServiceResult<CartItem>> GetCartItemAsync(CartItemQuery query);

        Task<ServiceResult<CartItem>> AddCartItemAsync(AddCartItemCommand command);

        Task<ServiceResult<CartItem>> UpdateCartItemAsync(UpdateCartItemCommand command);

        Task<ServiceResult<bool>> RemoveCartItemAsync(CartItemQuery query);

        Task<ServiceResult<CartItem>> AddImageAsync(AddCartItemImageCommand command);
    }
}
