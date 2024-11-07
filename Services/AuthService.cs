using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Jade.DTO;

namespace Jade.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> AssignRoleToUserAsync(string username, string roleName)
        {
            // Find the user by username
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                Console.WriteLine($"User '{username}' not found.");
                return false;
            }

            // Remove all existing roles
            var existingRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);

            if (!removeResult.Succeeded)
            {
                Console.WriteLine($"Failed to remove existing roles from user '{username}'. Errors: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
                return false;
            }

            // Assign the new role
            var addResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!addResult.Succeeded)
            {
                Console.WriteLine($"Failed to assign role '{roleName}' to user '{username}'. Errors: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
                return false;
            }

            return true;
        }

        public async Task<IdentityResult> RegisterUser(RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return IdentityResult.Failed(new IdentityError { Description = "User already exists!" });

            var user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<IdentityUser> AuthenticateUser(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return null;

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            return result ? user : null;
        }
    }
}
