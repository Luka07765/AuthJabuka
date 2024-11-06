using Jade.DTO;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Jade.Services
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUser(RegisterModel model);
        Task<IdentityUser> AuthenticateUser(LoginModel model);
    }
}