using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using OpenCart.Common;
using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;
using OpenCart.Operations.Commands;
using OpenCart.Operations.Queries;
using OpenCart.Repositories.Repositories.CartItemImageRepository;
using OpenCart.Repositories.Repositories.CartItemRepository;
using OpenCart.Repositories.Repositories.UserRepository;
using OpenCart.Services.Services.CartService;
using System.Linq.Expressions;

namespace OpenCart.Tests
{
    [TestClass]
    public class CartServiceTests
    {
        private ICartService _cartService;
        private Mock<ICartItemRepository> _cartItemRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ICartItemImageRepository> _imageRepositoryMock;
        private Mock<IValidator<CartItemImageDto>> _cartImageValidatorMock;
        private Mock<IValidator<CartItemDto>> _cartItemValidatorMock;
        private Mock<ILogger<CartService>> _loggerMock;

        public CartServiceTests()
        {
            _cartItemRepositoryMock = new Mock<ICartItemRepository>();
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _imageRepositoryMock = new Mock<ICartItemImageRepository>();
            _cartImageValidatorMock = new Mock<IValidator<CartItemImageDto>>();
            _cartItemValidatorMock = new Mock<IValidator<CartItemDto>>();
            _loggerMock = new Mock<ILogger<CartService>>();

            _cartService = new CartService(
                _loggerMock.Object,
                _cartItemRepositoryMock.Object,
                _mapperMock.Object,
                _userRepositoryMock.Object,
                _imageRepositoryMock.Object,
                _cartImageValidatorMock.Object,
                _cartItemValidatorMock.Object
            );
        }

        [TestMethod]
        public async Task AddCartItemAsync_ValidInput_ReturnsValidResponse()
        {
            // Arrange
            var command = new AddCartItemCommand
            {
                UserId = "user123",
                CartItem = new CartItemDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Product1",
                    Description = "Description1",
                    Price = 10.99m,
                    Quantity = 2
                }
            };

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                AuthProviderId = "user123"
            };

            var cartItemDto = command.CartItem;
            var cartItem = new CartItem
            {
                Id = cartItemDto.Id,
                Name = cartItemDto.Name,
                Description = cartItemDto.Description,
                Price = cartItemDto.Price,
                Quantity = cartItemDto.Quantity
            };

            var expectedResult = new ServiceResult<CartItemDto>
            {
                Response = new CartItemDto
                {
                    Id = cartItem.Id,
                    Name = cartItem.Name,
                    Description = cartItem.Description,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity
                }
            };
            _cartItemValidatorMock
                .Setup(validator => validator.ValidateAsync(command.CartItem, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            _mapperMock
                .Setup(mapper => mapper.Map<CartItem>(It.IsAny<CartItemDto>()))
                .Returns(cartItem);
            _mapperMock
                .Setup(mapper => mapper.Map<CartItemDto>(It.IsAny<CartItem>()))
                .Returns(cartItemDto);
            _cartItemRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<CartItem>()))
                .ReturnsAsync(cartItem);

            // Act
            var result = await _cartService.AddCartItemAsync(command);

            // Assert
            Assert.AreEqual(expectedResult.Response.Name, result.Response.Name);
            Assert.AreEqual(expectedResult.Response.Description, result.Response.Description);
            Assert.AreEqual(expectedResult.Response.Price, result.Response.Price);
            Assert.AreEqual(expectedResult.Response.Quantity, result.Response.Quantity);
            Assert.IsFalse(result.HasErrors);
        }
        [TestMethod]
        public async Task AddCartItemAsync_InvalidInput_ReturnsValidationError()
        {
            // Arrange
            var command = new AddCartItemCommand
            {
                UserId = "user123",
                CartItem = new CartItemDto
                {
                    // intentionally leaving out required fields
                }
            };

            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Price", "Price must be greater than zero"),
            };
            var validationResult = new ValidationResult(validationErrors);

            _cartItemValidatorMock
                .Setup(validator => validator.ValidateAsync(It.IsAny<CartItemDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _cartService.AddCartItemAsync(command);

            // Assert
            Assert.IsNotNull(result.Response);
            Assert.IsNull(result.Response.Description);
            Assert.AreEqual(result.Response.Id, Guid.Empty);
            Assert.AreEqual(result.Response.Quantity, 0);
            Assert.AreEqual(result.Response.Price, 0);
            Assert.AreEqual(validationErrors.Count, result.Errors.Count);
            Assert.AreEqual(validationErrors[0].ErrorMessage, result.Errors[0]);

        }

        [TestMethod]
        public async Task AddCartItemAsync_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new AddCartItemCommand
            {
                UserId = "user123",
                CartItem = new CartItemDto
                {
                    // intentionally leaving out required fields
                }
            };

            ApplicationUser user = null; // Simulate user not found

            _cartItemValidatorMock
                .Setup(validator => validator.ValidateAsync(It.IsAny<CartItemDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            // Act
            var result = await _cartService.AddCartItemAsync(command);

            // Assert
            Assert.IsNotNull(result.Response);
            Assert.IsNull(result.Response.Description);
            Assert.AreEqual(result.Response.Id, Guid.Empty);
            Assert.AreEqual(result.Response.Quantity, 0);
            Assert.AreEqual(result.Response.Price, 0);
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual("User not found", result.Errors[0]);

        }

        [TestMethod]
        public async Task AddCartItemAsync_CartItemNotAdded_ReturnsError()
        {
            // Arrange
            var command = new AddCartItemCommand
            {
                UserId = "user123",
                CartItem = new CartItemDto
                {
                    // intentionally leaving out required fields
                }
            };

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                AuthProviderId = "user123"
            };

            CartItem cartItem = null; // Simulate cart item not added

            _cartItemValidatorMock
                .Setup(validator => validator.ValidateAsync(It.IsAny<CartItemDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            _mapperMock
                .Setup(mapper => mapper.Map<CartItem>(It.IsAny<CartItemDto>()))
                .Returns(cartItem);

            _cartItemRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<CartItem>()))
                .ReturnsAsync(cartItem);

            // Act
            var result = await _cartService.AddCartItemAsync(command);

            // Assert
            Assert.IsNull(result.Response);
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual("An error occured at AddCartItemAsync when we tried to add a new cart item: UserID = user123", result.Errors[0]);

        }

        [TestMethod]
        public async Task GetCartItemAsync_ValidInput_ReturnsCartItem()
        {
            // Arrange
            var query = new CartItemQuery
            {
                UserId = "user123",
                CartItemId = Guid.NewGuid()
            };

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                AuthProviderId = "user123"
            };

            var cartItem = new CartItem
            {
                Id = query.CartItemId,
                Name = "Product1",
                Description = "Description1",
                Price = 10.99m,
                Quantity = 2
            };

            _userRepositoryMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            _cartItemRepositoryMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<CartItem, bool>>>()))
                .ReturnsAsync(cartItem);

            _mapperMock
                .Setup(mapper => mapper.Map<CartItemDto>(It.IsAny<CartItem>()))
                .Returns(new CartItemDto
                {
                    Id = cartItem.Id,
                    Name = cartItem.Name,
                    Description = cartItem.Description,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity
                });

            // Act
            var result = await _cartService.GetCartItemAsync(query);

            // Assert
            Assert.IsNotNull(result.Response);
            Assert.IsFalse(result.HasErrors);
            Assert.AreEqual(cartItem.Id, result.Response.Id);
            Assert.AreEqual(cartItem.Name, result.Response.Name);
            Assert.AreEqual(cartItem.Description, result.Response.Description);
            Assert.AreEqual(cartItem.Price, result.Response.Price);
            Assert.AreEqual(cartItem.Quantity, result.Response.Quantity);

        }


    }
}