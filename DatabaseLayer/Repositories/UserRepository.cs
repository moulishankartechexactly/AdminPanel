using DatabaseLayer.Data;
using DatabaseLayer.Interfaces;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DatabaseLayer.Repositories;

public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : base(context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Set<ApplicationUser>().CountAsync(cancellationToken);

    // Identity-backed operations
    public Task<IdentityResult> CreateWithPasswordAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default)
        => _userManager.CreateAsync(user, password);

    public Task<IdentityResult> UpdateIdentityAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => _userManager.UpdateAsync(user);

    public Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken = default)
        => _roleManager.RoleExistsAsync(roleName);

    public async Task EnsureRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken = default)
        => _userManager.AddToRoleAsync(user, roleName);

    public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => _userManager.GetRolesAsync(user);

    public Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles, CancellationToken cancellationToken = default)
        => _userManager.RemoveFromRolesAsync(user, roles);
}
