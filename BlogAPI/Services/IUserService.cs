using BlogAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Services
{
    public interface IUserService
    {
        Task<User?> GetUser();
    }
}