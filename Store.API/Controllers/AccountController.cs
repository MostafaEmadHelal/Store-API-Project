using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Data.Entities.IdentityEntities;
using Store.Service.HandleResponses;
using Store.Service.Services.TokenService;
using Store.Service.Services.UserService;
using Store.Service.Services.UserService.Dtos;
using System.Security.Claims;

namespace Store.API.Controllers
{

    public class AccountController : BaseController
    {
        private readonly IUserService userService;
        private readonly UserManager<AppUser> userManager;

        public AccountController(
            IUserService userService,
            UserManager<AppUser> userManager
            )
        {
            this.userService = userService;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Login(LoginDto input)
        {
            var user = await userService.Login(input);

            if (user is null)
                return Unauthorized(new CustomException(401));

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Register(RegisterDto input)
        {
            var user = await userService.Register(input);

            if (user is null)
                return BadRequest(new CustomException(400, "Email Already Exists"));

            return Ok(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUserDetails()
        {
            var email = User?.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.FindByEmailAsync(email);

            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
            };
        }
    }
}
