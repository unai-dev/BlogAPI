using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Services
{
    public interface IUserService
    {
        Task<IdentityUser?> GetUser();
    }
}