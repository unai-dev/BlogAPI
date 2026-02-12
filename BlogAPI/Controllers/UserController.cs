using BlogAPI.DTOs.UserAuth;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IUserService userService;

        public UserController(UserManager<IdentityUser> userManager, 
            IConfiguration configuration, SignInManager<IdentityUser> signInManager, IUserService userService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Register(UserCredentialsDTO userCredentialsDTO)
        {
            var user = new IdentityUser
            {
                UserName = userCredentialsDTO.Email,
                Email = userCredentialsDTO.Email
            };

            var result = await userManager.CreateAsync(user, userCredentialsDTO.Password);

            if (result.Succeeded)
            {

                var authResponse = await BuildToken(userCredentialsDTO);
                return authResponse ;

            } else
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }

                return ValidationProblem();
            }


        }

        [HttpPost("login")]
        [AllowAnonymous]
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

        [HttpGet("realod")]
        public async Task<ActionResult<AuthResponseDTO>> ReloadToken()
        {
            var user = await userService.GetUser();

            if (user is null) return NotFound();

            var userCredentialsDTO = new UserCredentialsDTO { Email = user.Email! };
            var response = await BuildToken(userCredentialsDTO);
            return response;
        }

        [HttpGet("me")]
       
        public async Task<ActionResult> GetMyId()
        {
            var user = await userService.GetUser();

            if (user is null) return Unauthorized();

            return Ok(new {user.Id});
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
