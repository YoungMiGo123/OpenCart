using OpenCart.Models.Entities;

namespace OpenCart.Services.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<ApplicationUser>> CreateAsync(Domain.User user);
        Task<bool> ExistAsync(string identityName);
    }
}
