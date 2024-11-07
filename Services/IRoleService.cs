namespace Jade.Services
{
    public interface IRoleService
    {
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
    }
}
