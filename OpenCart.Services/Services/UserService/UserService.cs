using OpenCart.Common;
using OpenCart.Models.Entities;
using OpenCart.Repositories.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCart.Services.Services.UserService
{
    public class UserService : IUserService
    {
        public UserService(IUserRepository userRepository)
        {

        }
        public Task<ServiceResult<ApplicationUser>> CreateUserAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExistAsync(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
