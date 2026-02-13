using BlogAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor contextAccessor;

        public UserService(UserManager<User> userManager, IHttpContextAccessor contextAccessor)
        {
            this.userManager = userManager;
            this.contextAccessor = contextAccessor;
        }

        public async Task<User?> GetUser()
        {
            var claimEmail = contextAccessor.HttpContext!.User.Claims.Where(x => x.Type == "email").FirstOrDefault();

            if (claimEmail is null)
            {
                return null;
            }

            var email = claimEmail.Value;
            return await userManager.FindByEmailAsync(email);
        }
    }
}
