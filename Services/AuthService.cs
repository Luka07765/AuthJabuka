using Jade.DTO;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Jade.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterUser(RegisterModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<SignInResult> LoginUser(LoginModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
        }
    }
}
