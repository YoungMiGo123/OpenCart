using OpenCart.Common;
using OpenCart.Models.Entities;

namespace OpenCart.Services.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResult<ApplicationUser>> CreateUserAsync(ApplicationUser user);

        Task<bool> UserExistAsync(string userName);
    }
}
