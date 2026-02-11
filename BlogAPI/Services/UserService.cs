using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor contextAccessor;

        public UserService(UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor)
        {
            this.userManager = userManager;
            this.contextAccessor = contextAccessor;
        }

        public async Task<IdentityUser?> GetUser()
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
