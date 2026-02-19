using AutoMapper;
using BlogAPI.Data;
using BlogAPI.DTOs.Claims;
using BlogAPI.DTOs.User;
using BlogAPI.DTOs.UserAuth;
using BlogAPI.Entities;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("v1/api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<User> signInManager;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly ITimeLimitedDataProtector protector;

        public UserController(UserManager<User> userManager,
            IConfiguration configuration, SignInManager<User> signInManager,
            IUserService userService, IMapper mapper, IDataProtectionProvider protectionProvider)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.userService = userService;
            this.mapper = mapper;

            protector = protectionProvider.CreateProtector("UserController").ToTimeLimitedDataProtector();
        }

        [HttpGet("get-token")]
        public ActionResult GetToken()
        {
            var textPlane = Guid.NewGuid().ToString();
            var token = protector.Protect(textPlane, TimeSpan.FromDays(2));
            var url = Url.RouteUrl("GetTokenListUsers", new { token }, "https");

            return Ok(url);
        }


        [HttpGet("{token}", Name = "GetTokenListUsers")]
        [AllowAnonymous]
        public async Task<ActionResult> GetTokenListUsers(string token)
        {
            try
            {
                protector.Unprotect(token);
            }
            catch
            {
                ModelState.AddModelError(nameof(token), "Token has expired");
                return ValidationProblem();
            }

            var users = await userManager.Users.Include(p => p.Posts).ToListAsync();
            var usersDTO = mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(usersDTO);
        }

        [HttpGet]
        [Authorize(Policy = "admin")]
        public async Task<IEnumerable<UserDTO>> Get()
        {
            var users = await userManager.Users.Include(p => p.Posts).ToListAsync();
            var usersDTO = mapper.Map<IEnumerable<UserDTO>>(users);
            return usersDTO;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetInfoUser()
        {
            var user = await userService.GetUser();

            if (user is null) return NotFound();

            return Ok(mapper.Map<UserDTO>(user));
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register(UserCredentialsDTO userCredentialsDTO)
        {
            var user = new User
            {
                UserName = userCredentialsDTO.Email,
                Email = userCredentialsDTO.Email
            };

            var result = await userManager.CreateAsync(user, userCredentialsDTO.Password!);

            if (result.Succeeded)
            {

                var authResponse = await BuildToken(userCredentialsDTO);
                return authResponse;

            }
            else
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }

                return ValidationProblem();
            }


        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(UserCredentialsDTO userCredentialsDTO)
        {
            var user = await userManager.FindByEmailAsync(userCredentialsDTO.Email);

            if (user is null)
            {
                return ReturnIncorrectLogin();
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, userCredentialsDTO.Password!, lockoutOnFailure: false);

            if (result.Succeeded) return await BuildToken(userCredentialsDTO);
            else return ReturnIncorrectLogin();


        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Put(UpdateUserDTO updateUserDTO)
        {
            var user = await userService.GetUser();

            if (user is null) return NotFound();

            user.Birthdate = updateUserDTO.Birthdate;

            await userManager.UpdateAsync(user);
            return NoContent();
        }

        [HttpGet("reload")]
        [Authorize]
        public async Task<ActionResult<AuthResponseDTO>> ReloadToken()
        {
            var user = await userService.GetUser();

            if (user is null) return NotFound();

            var userCredentialsDTO = new UserCredentialsDTO { Email = user.Email! };
            var response = await BuildToken(userCredentialsDTO);
            return response;
        }

        [HttpPost("add-admin")]
        [Authorize(Policy = "admin")]
        public async Task<ActionResult> AddAdmin(EditClaimDTO editClaimDTO)
        {
            var user = await userManager.FindByEmailAsync(editClaimDTO.Email);
            if (user is null) return NotFound();

            await userManager.AddClaimAsync(user, new Claim("admin", "true"));
            return NoContent();
        }

        [HttpPost("remove-admin")]
        [Authorize(Policy = "admin")]
        public async Task<ActionResult> RemoveAdmin(EditClaimDTO editClaimDTO)
        {
            var user = await userManager.FindByEmailAsync(editClaimDTO.Email);
            if (user is null) return NotFound();

            await userManager.RemoveClaimAsync(user, new Claim("admin", "true"));
            return NoContent();
        }

        private ActionResult ReturnIncorrectLogin()
        {
            ModelState.AddModelError(string.Empty, "Incorrect Login");
            return ValidationProblem();
        }

        private async Task<AuthResponseDTO> BuildToken(UserCredentialsDTO userCredentialsDTO)
        {
            var claims = new List<Claim>
            {
                new Claim("email", userCredentialsDTO.Email)
            };

            var user = await userManager.FindByEmailAsync(userCredentialsDTO.Email);
            var claimsDB = await userManager.GetClaimsAsync(user!);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddYears(1);

            var tokenSecurity = new JwtSecurityToken(
                issuer: null, audience: null, expires: expires, signingCredentials: credentials, claims: claims);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenSecurity);

            return new AuthResponseDTO
            {
                Token = token,
                Expires = expires
            };
        }
    }
}
