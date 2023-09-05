using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using OpenCart.Common;
using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;
using OpenCart.Operations.Commands;
using OpenCart.Operations.Queries;
using OpenCart.Repositories.Repositories.CartItemImageRepository;
using OpenCart.Repositories.Repositories.CartItemRepository;
using OpenCart.Repositories.Repositories.UserRepository;

namespace OpenCart.Services.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILogger<CartService> _logger;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ICartItemImageRepository _imageRepository;
        private readonly IValidator<CartItemImageDto> _cartImageValidator;
        private readonly IValidator<CartItemDto> _cartItemValidator;

        public CartService
        (
            ILogger<CartService> logger,
            ICartItemRepository cartItemRepository,
            IMapper mapper,
            IUserRepository userRepository,
            ICartItemImageRepository imageRepository,
            IValidator<CartItemImageDto> cartImageValidator,
            IValidator<CartItemDto> cartItemValidator
        )
        {
            _logger = logger;
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _cartImageValidator = cartImageValidator;
            _cartItemValidator = cartItemValidator;
        }
        public async Task<ServiceResult<CartItemDto>> AddCartItemAsync(AddCartItemCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command.UserId))
                {
                    throw new ArgumentNullException(nameof(command.UserId));
                }
                var validationResponse = await _cartItemValidator.ValidateAsync(command.CartItem);

                if (!validationResponse.IsValid)
                {
                    return FailedValidationResponse(validationResponse, $"Validation errors encountered at AddCartItemAsync while adding a cart item");
                }
                _logger.LogInformation($"Adding item for user {command.UserId}");

                var user = await _userRepository.FirstOrDefaultAsync(x => x.AuthProviderId == command.UserId);

                if (user == null)
                {
                    return NoUserFoundResponse(command.UserId);
                }
                var cartItem = _mapper.Map<CartItem>(command.CartItem);

                cartItem.UserId = user.Id;
                cartItem.CreatedDateTime = DateTime.UtcNow;
                cartItem.ModifiedDateTime = DateTime.UtcNow;
                cartItem.Id = Guid.NewGuid();

                var result = await _cartItemRepository.AddAsync(cartItem);

                if (result == null)
                {
                    _logger.LogError($"Failed to add item for user {command.UserId}");
                    return new ServiceResult<CartItemDto>
                    {
                        Response = new CartItemDto(),
                        Errors = new List<string> { $"Failed to add cart item for user {command.UserId}" }
                    };
                }

                var resultDto = _mapper.Map<CartItemDto>(result);
                _logger.LogInformation($"Item added successfully for user {command.UserId}, cart item ID {command?.CartItem?.Id}");
                return new ServiceResult<CartItemDto>
                {
                    Response = resultDto
                };
            }
            catch (Exception ex)
            {
                var userId = string.IsNullOrWhiteSpace(command.UserId) ? "No user found" : command.UserId;
                _logger.LogError(ex, $"An error occured at AddCartItemAsync when we tried to add a new cart item: UserID = {userId} Cart Item {command.CartItem}");
                return new ServiceResult<CartItemDto>()
                {
                    Errors = new List<string>()
                    {
                        $"An error occured at AddCartItemAsync when we tried to add a new cart item: UserID = {userId}"
                    }
                };
            }

        }

        public async Task<ServiceResult<CartItemDto>> AddImageAsync(AddCartItemImageCommand command)
        {
            try
            {
                ValidateParameters(command.UserId, command.CartItemId);

                _logger.LogInformation($"Adding image for user {command.UserId}, cart item ID {command.CartItemId}");

                var validationResult = await _cartImageValidator.ValidateAsync(command.CartItemImage);

                if (!validationResult.IsValid)
                {
                    return FailedValidationResponse(validationResult, $"Validation errors encountered while adding image for user {command.UserId}, cart item ID {command.CartItemId}");
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
                    return new ServiceResult<CartItemDto>("Cart item not found");
                }

                var cartItemImage = _mapper.Map<CartItemImage>(command.CartItemImage);
                cartItemImage.CreatedDateTime = DateTime.UtcNow;
                cartItemImage.ModifiedDateTime = DateTime.UtcNow;
                cartItemImage.Id = Guid.NewGuid();
                cartItem.CartItemImages.Add(cartItemImage);

                var result = await _imageRepository.UpdateAsync(cartItemImage);

                if (result == null)
                {
                    _logger.LogError($"Failed to add image for user {command.UserId}, cart item ID {command.CartItemId}");
                    return new ServiceResult<CartItemDto>("Failed to add image") { Response = new CartItemDto() };
                }

                var resultDto = _mapper.Map<CartItemDto>(result);
                _logger.LogError($"Failed to add image for user {user.Id}, cart item ID {command.CartItemId}");
                return new ServiceResult<CartItemDto>(resultDto);
            }
            catch (Exception ex)
            {
                var userId = string.IsNullOrWhiteSpace(command.UserId) ? "No user found" : command.UserId;

                _logger.LogError(ex, $"An error occured at AddImageAsync when we tried to add a new cart item image: UserID = {userId} Cart Item Image {command.CartItemId}");

                return new ServiceResult<CartItemDto>()
                {
                    Errors = new List<string>()
                    {
                        $"An error occured at AddImageAsync when we tried to add a new cart item image: UserID = {userId} Cart Item Image {command.CartItemId}"
                    }
                };
            }

        }

        public async Task<ServiceResult<CartItemDto>> GetCartItemAsync(CartItemQuery query)
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
                    return new ServiceResult<CartItemDto>()
                    {
                        Errors = new List<string> { "Cart item not found" }
                    };
                }

                _logger.LogInformation($"Retrieved details for cart item ID {query.CartItemId} for user {user.Id}");
                var resultDto = _mapper.Map<CartItemDto>(cartItem);
                return new ServiceResult<CartItemDto>(resultDto);

            }
            catch (Exception ex)
            {
                var userId = string.IsNullOrWhiteSpace(query.UserId) ? "No user found" : query.UserId;

                _logger.LogError(ex, $"An error occured at GetCartItemAsync: UserID = {userId} Cart Item {query.CartItemId}");

                return new ServiceResult<CartItemDto>()
                {
                    Response = new CartItemDto(),
                    Errors = new List<string>()
                    {
                        $"An error occured at GetCartItemAsync: UserID = {userId} Cart Item {query.CartItemId}"
                    }
                };
            }

        }

        public async Task<ServiceResult<PaginationResponse<CartItemDto>>> GetCartItemsAsync(string userId)
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

                    return new ServiceResult<PaginationResponse<CartItemDto>>()
                    {
                        Response = new PaginationResponse<CartItemDto>() { Data = new List<CartItemDto>() },
                        Errors = new List<string> { "User not found" }
                    };
                }

                var cartItems = await _cartItemRepository.GetAllPagedAsync(filter: x => x.UserId == user.Id);

                _logger.LogInformation($"Retrieved {cartItems.TotalCount} items for user {userId}");

                var cartItemDtos = _mapper.Map<List<CartItemDto>>(cartItems.Data);

                return new ServiceResult<PaginationResponse<CartItemDto>>()
                {
                    Response = new PaginationResponse<CartItemDto>
                    {
                        Data = cartItemDtos,
                        Page = cartItems.Page,
                        PageSize = cartItems.PageSize,
                        TotalCount = cartItems.TotalCount
                    }
                };

            }
            catch (Exception ex)
            {
                userId = string.IsNullOrWhiteSpace(userId) ? "No user found" : userId;

                _logger.LogError(ex, $"An error occured at GetCartItemsAsync when we tried to add a new cart item: UserID = {userId}");

                return new ServiceResult<PaginationResponse<CartItemDto>>()
                {
                    Response = new PaginationResponse<CartItemDto>() { Data = new List<CartItemDto>() },
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
                var userId = string.IsNullOrWhiteSpace(query.UserId) ? "No user found" : query.UserId;

                _logger.LogError(ex, $"An error occured at RemoveCartItemAsync when we tried to remove a cart item: UserID = {userId} Cart Item {query.CartItemId}");

                return new ServiceResult<bool>(false)
                {
                    Errors = new List<string>()
                    {
                        $"An error occured at AddCartItemAsync when we tried to remove a cart item: UserID = {userId}"
                    }
                };
            }
        }

        public async Task<ServiceResult<CartItemDto>> UpdateCartItemAsync(UpdateCartItemCommand command)
        {
            try
            {
                ValidateParameters(command.UserId, command.CartItemId);

                _logger.LogInformation($"Updating item for user {command.UserId}, cart item ID {command.CartItemId}");

                var validationResponse = await _cartItemValidator.ValidateAsync(command.CartItem);
                if (!validationResponse.IsValid)
                {
                    return FailedValidationResponse(validationResponse, "Failed at UpdateCartItemAsync with validation errors");
                }

                var user = await _userRepository.FirstOrDefaultAsync(x => x.AuthProviderId == command.UserId);

                if (user == null)
                {
                    return NoUserFoundResponse(command.UserId);
                }

                var cartItem = await _cartItemRepository.FirstOrDefaultAsync(x => x.Id == command.CartItemId && x.UserId == user.Id);

                if (cartItem == null)
                {
                    _logger.LogWarning($"Cart item with ID {command.CartItemId} not found for user {user.Id}");
                    return new ServiceResult<CartItemDto>("Cart item not found");
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
                    return new ServiceResult<CartItemDto>
                    {
                        Response = new CartItemDto(),
                        Errors = new List<string> { errorMessage }
                    };

                }
                var resultDto = _mapper.Map<CartItemDto>(result);
                _logger.LogInformation($"Successfully updated cart item for user {user.Id}, cart item ID {command.CartItemId}");
                return new ServiceResult<CartItemDto>(resultDto);

            }
            catch (Exception ex)
            {
                var userId = string.IsNullOrWhiteSpace(command.UserId) ? "No user found" : command.UserId;

                _logger.LogError(ex, $"An error occured at UpdateCartItemAsync when we tried to update a cart: UserID = {userId} Cart Item {command.CartItem}");

                return new ServiceResult<CartItemDto>()
                {
                    Errors = new List<string>()
                    {
                        $"An error occured at UpdateCartItemAsync when we tried to update a cart: UserID = {userId}"
                    }
                };
            }
            throw new NotImplementedException();
        }

        private ServiceResult<CartItemDto> FailedValidationResponse(ValidationResult validationResponse, string message)
        {
            var validationErrors = validationResponse.Errors.Select(x => x.ErrorMessage).ToList();
            _logger.LogWarning($"{message}: {validationErrors}");
            return new ServiceResult<CartItemDto>() { Response = new CartItemDto(), Errors = validationErrors };
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

        private ServiceResult<CartItemDto> NoUserFoundResponse(string userId)
        {
            _logger.LogWarning($"User with ID {userId} not found");
            return new ServiceResult<CartItemDto>
            {
                Response = new CartItemDto(),
                Errors = new List<string> { "User not found" }
            };
        }

    }
}
