using Jade.DTO;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Jade.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUser(RegisterModel model);
        Task<SignInResult> LoginUser(LoginModel model);
    }
}
