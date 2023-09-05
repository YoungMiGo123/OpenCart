using Microsoft.AspNetCore.Mvc;
using OpenCart.Models.Entities;
using OpenCart.Services.Services.UserService;
using System.Security.Claims;

namespace OpenCart.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;
        private string _currentUserId;
        public BaseController(IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        private string GetOrCreateUserReturnsUserId()
        {
            if (_contextAccessor?.HttpContext?.User == null)
            {
                return string.Empty;
            }

            if (_contextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                var claims = _contextAccessor?.HttpContext?.User?.Claims;

                var claimsResponse = claims as Claim[] ?? claims?.ToArray();
                var username = claimsResponse?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username))
                {
                    return string.Empty;
                }

                if (_userService.UserExistAsync(username).Result)
                {
                    _currentUserId = claimsResponse?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                    return _currentUserId;
                }
              
                var names = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value?.Split(' ');

                // create a user from claims
                var user = new ApplicationUser()
                {
                    Email = claimsResponse?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? string.Empty,
                    Username = username,
                    FirstName = names?.FirstOrDefault() ?? string.Empty,
                    LastName = names?.LastOrDefault() ?? string.Empty,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow,
                    AuthProviderId = claimsResponse?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty
                };

                var result = _userService.CreateUserAsync(user).Result;

                if (!result.HasErrors)
                {
                    return result.Response.AuthProviderId;
                }

            }

            return string.Empty;
        }
        public string? CurrentUserId => !string.IsNullOrWhiteSpace(_currentUserId) ? _currentUserId : GetOrCreateUserReturnsUserId();
    }
}
