using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using OpenCart.Models.Entities;
using OpenCart.Repositories.Repositories.UserRepository;
using OpenCart.Services.Services.UserService;
using System.Linq.Expressions;

namespace OpenCart.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private readonly Mock<ILogger<UserService>> _loggerMock = new Mock<ILogger<UserService>>();
        private readonly Mock<IValidator<ApplicationUser>> _validatorMock = new Mock<IValidator<ApplicationUser>>();

        public UserServiceTests()
        {
            _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object, _validatorMock.Object);
        }

        [TestMethod]
        public async Task CreateUserAsync_ValidUser_ReturnsValidResponse()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
            };

            _validatorMock
                .Setup(validator => validator.Validate(It.IsAny<ApplicationUser>()))
                .Returns(new ValidationResult());

            _userRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.CreateUserAsync(user);

            // Assert
            Assert.IsNotNull(result.Response);
            Assert.IsFalse(result.HasErrors);
            Assert.AreEqual(user.Id, result.Response.Id);
        }

        [TestMethod]
        public async Task CreateUserAsync_InvalidUser_ReturnsValidationErrors()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
            };

            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Username", "Username is required."),
            };

            _validatorMock
                .Setup(validator => validator.Validate(It.IsAny<ApplicationUser>()))
                .Returns(new ValidationResult(validationErrors));

            // Act
            var result = await _userService.CreateUserAsync(user);

            // Assert
            Assert.IsNull(result.Response);
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(validationErrors.Count, result.Errors.Count);

        }

        [TestMethod]
        public async Task UserExistAsync_UserExists_ReturnsTrue()
        {
            // Arrange
            var userName = "existingUser";

            _userRepositoryMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(new ApplicationUser());

            // Act
            var exists = await _userService.UserExistAsync(userName);

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task UserExistAsync_UserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var userName = "nonExistingUser";

            _userRepositoryMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var exists = await _userService.UserExistAsync(userName);

            // Assert
            Assert.IsFalse(exists);
        }
    }
}
