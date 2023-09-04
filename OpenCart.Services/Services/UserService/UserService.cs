﻿using Microsoft.Extensions.Logging;
using OpenCart.Common;
using OpenCart.Models.Entities;
using OpenCart.Repositories.Repositories.UserRepository;
using OpenCart.Services.Services.Validators;

namespace OpenCart.Services.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly UserValidator _validations;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, UserValidator validations)
        {
            _userRepository = userRepository;
            _logger = logger;
            _validations = validations;
        }
        public async Task<ServiceResult<ApplicationUser>> CreateUserAsync(ApplicationUser user)
        {
            try
            {
                var validationResponse = _validations.Validate(user);

                if (!validationResponse.IsValid)
                {
                    var validationErrors = validationResponse.Errors.Select(x => x.ErrorMessage).ToList();
                    _logger.LogWarning("Validation errors encountered while adding a user {user}: {ValidationErrors}", user, validationErrors);
                    return new ServiceResult<ApplicationUser>() { Errors = validationErrors };
                }

                var response = await _userRepository.AddAsync(user);
                _logger.LogInformation("User added successfully {User}", response);

                return new ServiceResult<ApplicationUser>(response);

            }
            catch (Exception ex)
            {
                _logger.LogError($"We failed to create a new user at CreateUserAsync. User ID = {user.Id}. Error = {ex}");

                return new ServiceResult<ApplicationUser>()
                {
                    Errors = new List<string>()
                    {
                        $"An error occurred while adding a user {user.Id}"
                    }
                };
            }

        }

        public Task<bool> UserExistAsync(string userName)
        {
            return Task.FromResult(_userRepository.Get(filter: x => x.Username == userName).Any());
        }
    }
}
