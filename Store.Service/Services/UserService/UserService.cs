using Microsoft.AspNetCore.Identity;
using Store.Data.Entities.IdentityEntities;
using Store.Service.Services.TokenService;
using Store.Service.Services.UserService.Dtos;

namespace Store.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;

        public UserService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        public async Task<UserDto> Login(LoginDto input)
        {
            var user = await userManager.FindByEmailAsync(input.Email);

            if (user is null)
                return null;

            var result = await signInManager.CheckPasswordSignInAsync(user, input.Password, false);

            if (!result.Succeeded)
                throw new Exception("Login Failed");

            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = tokenService.GenerateToken(user)
            };
        }

        public async Task<UserDto> Register(RegisterDto input)
        {
            var user = await userManager.FindByEmailAsync(input.Email);

            if (user is not null)
                return null;

            var appUser = new AppUser
            {
                DisplayName = input.DisplayName,
                Email = input.Email,
                UserName = input.DisplayName
            };

            var result = await userManager.CreateAsync(appUser, input.Password);

            if (!result.Succeeded)
                throw new Exception(result.Errors.Select(x => x.Description).FirstOrDefault());

            return new UserDto
            {
                Email = input.Email,
                DisplayName = input.DisplayName,
                Token = tokenService.GenerateToken(appUser)
            };

        }
    }
}
