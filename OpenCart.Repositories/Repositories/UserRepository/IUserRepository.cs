using OpenCart.Models.Entities;
using OpenCart.Repositories.Repositories.GenericRepository;

namespace OpenCart.Repositories.Repositories.UserRepository
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        // Additional methods specific to the UserRepo, if any.
    }
}
