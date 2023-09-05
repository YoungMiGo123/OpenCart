using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;
using OpenCart.Operations.Commands;
using OpenCart.Operations.Queries;
using OpenCart.Services.Services.CartService;
using OpenCart.Services.Services.UserService;

namespace OpenCart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenCartController : BaseController
    {
        private readonly ICartService _cartService;

        public OpenCartController(IHttpContextAccessor contextAccessor, IUserService userService, ICartService cartService) : base(contextAccessor, userService)
        {
            _cartService = cartService;
        }


        [HttpGet]
        [Route("GetCartItems")]
        public async Task<IActionResult> GetCartItems()
        {
            var result = await _cartService.GetCartItemsAsync(CurrentUserId);
            if (result.HasErrors)
            {
                return BadRequest(string.Join(",", result.Errors));
            }
            return Ok(result.Response);
        }

        [HttpGet]
        [Route("GetCartItem/{cartItemId}")]
        public async Task<IActionResult> GetCartItem(Guid cartItemId)
        {
            var result = await _cartService.GetCartItemAsync(new CartItemQuery
            {
                CartItemId = cartItemId,
                UserId = CurrentUserId
            });
            if (result.HasErrors)
            {
                return BadRequest(string.Join(",", result.Errors));
            }
            return Ok(result.Response);
        }

        [HttpPost]
        [Route("AddCartItem")]
        public async Task<IActionResult> AddCartItem([FromBody] CartItemDto cartItem)
        {
            var result = await _cartService.AddCartItemAsync(new AddCartItemCommand
            {
                UserId = CurrentUserId, 
                CartItem = cartItem
            });
            if (result.HasErrors)
            {
                return BadRequest(string.Join(",", result.Errors));
            }
            return Ok(result.Response);
        }

        [HttpPut]
        [Route("UpdateCartItem")]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItemDto cartItem)
        {
            var result = await _cartService.UpdateCartItemAsync(new UpdateCartItemCommand
            {
                CartItem = cartItem,
                UserId = CurrentUserId,
                CartItemId = cartItem.Id
            });
            if (result.HasErrors)
            {
                return BadRequest(string.Join(",", result.Errors));
            }
            return Ok(result.Response);
        }

        [HttpDelete]
        [Route("RemoveCartItem/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
        {
            var result = await _cartService.RemoveCartItemAsync(new CartItemQuery
            {
                UserId = CurrentUserId,
                CartItemId = cartItemId
            });
            if (result.HasErrors)
            {
                return BadRequest(string.Join(",", result.Errors));
            }
            return Ok(result.Response);
        }

        [HttpPost]
        [Route("AddImageToCartItem/{cartItemId}")]
        public async Task<IActionResult> AddImageToCartItem(Guid cartItemId, [FromBody] CartItemImageDto cartItemImage)
        {
            var result = await _cartService.AddImageAsync(new AddCartItemImageCommand
            {
                UserId = CurrentUserId,
                CartItemId = cartItemId,
                CartItemImage = cartItemImage
            });

            if (result.HasErrors)
            {
                return BadRequest(string.Join(",", result.Errors));
            }
            return Ok(result.Response);
        }
    }
}
