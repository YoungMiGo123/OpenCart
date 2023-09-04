using OpenCart.Infrastructure.Context;
using OpenCart.Models.Entities;

namespace OpenCart.Repositories.Repositories.UserRepository
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(OpenCartDbContext dbContext) : base(dbContext)
        {
        }
    }
}
