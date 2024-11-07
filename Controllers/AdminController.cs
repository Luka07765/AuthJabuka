using System.Threading.Tasks;
using Jade.DTO;
using Jade.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jade.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(IUserService userService, IRoleService roleService, UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _roleService = roleService;
            _userManager = userManager;
        }

        // POST: api/Admin/AssignRole
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if the role exists
            var roleExists = await _roleService.RoleExistsAsync(model.RoleName);
            if (!roleExists)
                return BadRequest("Role does not exist.");

            // Find the user
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return NotFound($"User '{model.Username}' not found.");

            // Remove all existing roles from the user
            var existingRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
            if (!removeResult.Succeeded)
                return BadRequest("Failed to remove existing roles from user.");

            // Assign the new role
            var addResult = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (!addResult.Succeeded)
                return BadRequest($"Failed to assign role '{model.RoleName}' to user.");

            return Ok("Role assigned successfully.");
        }
    }
}
