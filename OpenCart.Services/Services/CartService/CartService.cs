using Microsoft.Extensions.Logging;
using OpenCart.Common;
using OpenCart.Models.Entities;
using OpenCart.Operations.Commands;
using OpenCart.Operations.Queries;
using OpenCart.Repositories.Repositories.CartItemImageRepository;
using OpenCart.Repositories.Repositories.CartItemRepository;
using OpenCart.Repositories.Repositories.UserRepository;
using OpenCart.Services.Services.Validators;

namespace OpenCart.Services.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILogger<CartService> _logger;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICartItemImageRepository _imageRepository;
        private readonly CartImageValidator _cartImageValidator;

        public CartService(ILogger<CartService> logger, ICartItemRepository cartItemRepository, IUserRepository userRepository, ICartItemImageRepository imageRepository, CartImageValidator cartImageValidator)
        {
            _logger = logger;
            _cartItemRepository = cartItemRepository;
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _cartImageValidator = cartImageValidator;
        }
        public async Task<ServiceResult<CartItem>> AddCartItemAsync(AddCartItemCommand command)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(command.UserId))
                {
                    throw new ArgumentNullException(nameof(command.UserId));
                }

                _logger.LogInformation($"Adding item for user {command.UserId}");

                var user = await _userRepository.FirstOrDefaultAsync(x => x.AuthProviderId == command.UserId);

                if (user == null)
                {
                    return NoUserFoundResponse(command.UserId);
                }
                command.CartItem.UserId = user.Id;
                var result = await _cartItemRepository.AddAsync(command.CartItem);

                if (result == null)
                {
                    _logger.LogError($"Failed to add item for user {command.UserId}");
                    return new ServiceResult<CartItem>
                    {
                        Response = new CartItem(),
                        Errors = new List<string> { $"Failed to add cart item for user {command.UserId}" }
                    };
                }
             
                _logger.LogInformation($"Item added successfully for user {command.UserId}, cart item ID {command?.CartItem?.Id}");
                return new ServiceResult<CartItem>
                {
                    Response = result
                };
            }  
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured at AddCartItemAsync when we tried to add a new cart item: UserID = {command.UserId} Cart Item {command.CartItem}");
                
                return new ServiceResult<CartItem>()
                {
                    Errors = new List<string>()
                    {
                        $"An error occured at AddCartItemAsync when we tried to add a new cart item: UserID = {command.UserId}"
                    }
                };
            }
          
        }

        public async Task<ServiceResult<CartItem>> AddImageAsync(AddCartItemImageCommand command)
        {
            try
            {
                ValidateParameters(command.UserId, command.CartItemId);

                _logger.LogInformation($"Adding image for user {command.UserId}, cart item ID {command.CartItemId}");

                var validationResult = await _cartImageValidator.ValidateAsync(command.CartItemImage);

                if (!validationResult.IsValid)
                {
                    var validationErrors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                    _logger.LogWarning($"Validation errors encountered while adding image for user {command.UserId}, cart item ID {command.CartItemId}: {string.Join(", ", validationErrors)}");
                    
                    return new ServiceResult<CartItem>
                    {
                        Errors = validationErrors
                    };
                }

                var user = await _userRepository.FirstOrDefaultAsync(x => x.AuthProviderId == command.UserId);

                if (user == null)
                {
                    return NoUserFoundResponse(command.UserId);
                }

                var cartItem = await _cartItemRepository.FirstOrDefaultAsync(x => x.Id == command.CartItemId && x.UserId == user.Id);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item with ID {command.CartItemId} not found for user {command.UserId}");
                    return new ServiceResult<CartItem>("Cart item not found");
                }

                var cartItemImage = command.CartItemImage;
                cartItem.CartItemImages.Add(cartItemImage);

                var result = await _imageRepository.UpdateAsync(cartItemImage);

                if (result == null)
                {
                    _logger.LogError($"Failed to add image for user {command.UserId}, cart item ID {command.CartItemId}");
                    return new ServiceResult<CartItem>("Failed to add image");
                }
                
                _logger.LogError($"Failed to add image for user {user.Id}, cart item ID {command.CartItemId}");
                return new ServiceResult<CartItem>(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured at AddImageAsync when we tried to add a new cart item image: UserID = {command.UserId} Cart Item Image {command.CartItemId}");

                return new ServiceResult<CartItem>()
                {
                    Errors = new List<string>()
                    {
                        $"An error occured at AddImageAsync when we tried to add a new cart item image: UserID = {command.UserId} Cart Item Image {command.CartItemId}"
                    }
                };
            }
     
        }

        public async Task<ServiceResult<CartItem>> GetCartItemAsync(CartItemQuery query)
        {
            try
            {
                ValidateParameters(query.UserId, query.CartItemId);

                _logger.LogInformation($"Getting item details for user {query.UserId}, cart item ID {query.CartItemId}");

                var user = await _userRepository.FirstOrDefaultAsync(x => x.AuthProviderId == query.UserId);

                if (user == null)
                {
                    return NoUserFoundResponse(query.UserId);
                }

                var cartItem = await _cartItemRepository.FirstOrDefaultAsync(x => x.Id == query.CartItemId && x.UserId == user.Id);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item with ID {query.CartItemId} not found for user {user.Id}");
                    return new ServiceResult<CartItem>()
                    {
                        Errors = new List<string> { "Cart item not found" }
                    };
                }

                _logger.LogInformation($"Retrieved details for cart item ID {query.CartItemId} for user {user.Id}");

                return new ServiceResult<CartItem>(cartItem);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured at GetCartItemAsync: UserID = {query.UserId} Cart Item {query.CartItemId}");

                return new ServiceResult<CartItem>()
                {
                    Response = new CartItem(),
                    Errors = new List<string>()
                    {
                        $"An error occured at GetCartItemAsync: UserID = {query.UserId} Cart Item {query.CartItemId}"
                    }
                };
            }
       
        }

        public async Task<ServiceResult<PaginationResponse<CartItem>>> GetCartItemsAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                _logger.LogInformation($"Getting cart items for user {userId}");

                var user = await _userRepository.FirstOrDefaultAsync(x => x.AuthProviderId == userId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found");

                    return new ServiceResult<PaginationResponse<CartItem>>()
                    {
                        Response = new PaginationResponse<CartItem>() { Data = new List<CartItem>() },
                        Errors = new List<string> { "User not found" }
                    };
                }

                var cartItems = await _cartItemRepository.GetAllPagedAsync(filter: x => x.UserId == user.Id);

                _logger.LogInformation($"Retrieved {cartItems.TotalCount} items for user {userId}");

                return new ServiceResult<PaginationResponse<CartItem>>()
                {
                    Response = cartItems,
                    Errors = new List<string> { "User not found" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured at GetCartItemsAsync when we tried to add a new cart item: UserID = {userId}");

                return new ServiceResult<PaginationResponse<CartItem>> ()
                {
                    Response = new PaginationResponse<CartItem>() { Data = new List<CartItem>() },
                    Errors = new List<string>()
                    {
                        $"An error occured at GetCartItemsAsync when we tried to add a new cart item: UserID = {userId}"
                    }
                };
            }

        }

        public async Task<ServiceResult<bool>> RemoveCartItemAsync(CartItemQuery query)
        {
            try
            {
                ValidateParameters(query.UserId, query.CartItemId);

                _logger.LogInformation($"Removing item for user {query.UserId}, cart item ID {query.CartItemId}");

                var user = await _userRepository.FirstOrDefaultAsync(x => x.AuthProviderId == query.UserId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {query.UserId} not found");
                    return new ServiceResult<bool>("User not found");
                }

                var cartItem = await _cartItemRepository.FirstOrDefaultAsync(x => x.Id == query.CartItemId && x.UserId == user.Id);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item with ID {query.CartItemId} not found for user {query.UserId}");
                    return new ServiceResult<bool>("Cart item not found");
                }

                var result = await _cartItemRepository.DeleteAsync(cartItem.Id);

                _logger.LogInformation($"Item removed successfully for user {query.UserId}, cart item ID {query.CartItemId}");

                return new ServiceResult<bool>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured at RemoveCartItemAsync when we tried to remove a cart item: UserID = {query.UserId} Cart Item {query.CartItemId}");

                return new ServiceResult<bool>(false)
                {
                    Errors = new List<string>()
                    {
                        $"An error occured at AddCartItemAsync when we tried to remove a cart item: UserID = {query.UserId}"
                    }
                };
            }
        }

        public async Task<ServiceResult<CartItem>> UpdateCartItemAsync(UpdateCartItemCommand command)
        {
            try
            {
                ValidateParameters(command.UserId, command.CartItemId);

                _logger.LogInformation($"Updating item for user {command.UserId}, cart item ID {command.CartItemId}");

                var user = await _userRepository.FirstOrDefaultAsync(x => x.AuthProviderId == command.UserId);

                if (user == null)
                {
                    return NoUserFoundResponse(command.UserId);
                }

                var cartItem = await _cartItemRepository.FirstOrDefaultAsync(x => x.Id == command.CartItemId && x.UserId == user.Id);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item with ID {command.CartItemId} not found for user {user.Id}");
                    return new ServiceResult<CartItem>("Cart item not found");
                }

                var cartItemInput = command.CartItem;

                cartItem.Name = cartItemInput.Name;
                cartItem.Description = cartItemInput.Description;
                cartItem.Price = cartItemInput.Price;
                cartItem.Quantity = cartItemInput.Quantity;

                var result = await _cartItemRepository.UpdateAsync(cartItem);

                if (result == null)
                {
                    var errorMessage = $"Failed to update item for user {user.Id}, cart item ID {command.CartItemId}";
                    _logger.LogError(errorMessage);
                    return new ServiceResult<CartItem>
                    {
                        Response = new CartItem(),
                        Errors = new List<string> { errorMessage }
                    };

                }
                _logger.LogInformation($"Successfully updated cart item for user {user.Id}, cart item ID {command.CartItemId}");
                return new ServiceResult<CartItem>(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured at UpdateCartItemAsync when we tried to update a cart: UserID = {command.UserId} Cart Item {command.CartItem}");

                return new ServiceResult<CartItem>()
                {
                    Errors = new List<string>()
                    {
                        $"An error occured at UpdateCartItemAsync when we tried to update a cart: UserID = {command.UserId}"
                    }
                };
            }
            throw new NotImplementedException();
        }
        private void ValidateParameters(string userId, Guid cartItemId)
        {
            _logger.LogDebug("Validating parameters for adding image: user ID {UserId}, cart item ID {CartItemId}", userId, cartItemId);

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("User ID is null or empty");
                throw new ArgumentNullException(nameof(userId));
            }

            if (Guid.Empty == cartItemId)
            {
                _logger.LogError("Cart item ID is not valid: {CartItemId}", cartItemId);
                throw new ArgumentOutOfRangeException(nameof(cartItemId));
            }
        }

        private ServiceResult<CartItem> NoUserFoundResponse(string userId)
        {
            _logger.LogWarning($"User with ID {userId} not found");
            return new ServiceResult<CartItem>
            {
                Response = new CartItem(),
                Errors = new List<string> { "User not found" }
            };
        }

    }
}
