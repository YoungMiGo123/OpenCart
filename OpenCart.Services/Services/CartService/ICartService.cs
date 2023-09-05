using OpenCart.Common;
using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;
using OpenCart.Operations.Commands;
using OpenCart.Operations.Queries;

namespace OpenCart.Services.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResult<PaginationResponse<CartItemDto>>> GetCartItemsAsync(string userId);

        Task<ServiceResult<CartItemDto>> GetCartItemAsync(CartItemQuery query);

        Task<ServiceResult<CartItemDto>> AddCartItemAsync(AddCartItemCommand command);

        Task<ServiceResult<CartItemDto>> UpdateCartItemAsync(UpdateCartItemCommand command);

        Task<ServiceResult<bool>> RemoveCartItemAsync(CartItemQuery query);

        Task<ServiceResult<CartItemDto>> AddImageAsync(AddCartItemImageCommand command);
    }
}
