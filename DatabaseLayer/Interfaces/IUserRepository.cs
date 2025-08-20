using Entity.Models;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DatabaseLayer.Interfaces;

public interface IUserRepository : IGenericRepository<ApplicationUser>
{
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    // Identity-backed operations
    Task<IdentityResult> CreateWithPasswordAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdateIdentityAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken = default);
    Task EnsureRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken = default);
    Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles, CancellationToken cancellationToken = default);
}
