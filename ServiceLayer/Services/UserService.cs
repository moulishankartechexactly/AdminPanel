using DatabaseLayer.Interfaces;
using Entity.Models;
using Model.Dtos;
using ServiceLayer.Interfaces;

namespace ServiceLayer.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users)
    {
        _users = users;
    }

    public async Task<UserDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var entity = await _users.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _users.GetAllAsync(cancellationToken);
        return list.Select(ToDto).ToList();
    }

    public async Task<UserDto> CreateAsync(UserDto user, CancellationToken cancellationToken = default)
    {
        // If password is provided, create via Identity to ensure Id, Normalized fields, and PasswordHash are set
        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            var entity = new ApplicationUser
            {
                UserName = user.UserName!,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true ? UserRole.Admin : UserRole.Manager
            };

            var result = await _users.CreateWithPasswordAsync(entity, user.Password!, cancellationToken);
            if (!result.Succeeded)
            {
                var msg = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {msg}");
            }

            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                await _users.EnsureRoleAsync(user.Role!, cancellationToken);
                await _users.AddToRoleAsync(entity, user.Role!, cancellationToken);
            }

            return ToDto(entity);
        }

        // Legacy path (no password): create via repository only (cannot login without later password set)
        var legacy = FromDto(user);
        await _users.AddAsync(legacy, cancellationToken);
        await _users.SaveChangesAsync(cancellationToken);
        return ToDto(legacy);
    }

    public async Task UpdateAsync(UserDto user, CancellationToken cancellationToken = default)
    {
        var existing = await _users.GetByIdAsync(user.Id, cancellationToken);
        if (existing is null) return;
        existing.Email = user.Email;
        existing.UserName = user.UserName;
        existing.PhoneNumber = user.PhoneNumber;

        if (!string.IsNullOrWhiteSpace(user.Role))
        {
            existing.Role = user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ? UserRole.Admin : UserRole.Manager;
        }

        // Update via Identity to maintain normalized fields
        var updateResult = await _users.UpdateIdentityAsync(existing, cancellationToken);
        if (!updateResult.Succeeded)
        {
            var msg = string.Join("; ", updateResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update user: {msg}");
        }

        // Manage role membership: ensure only the selected role is applied (Admin/Manager)
        if (!string.IsNullOrWhiteSpace(user.Role))
        {
            var currentRoles = await _users.GetRolesAsync(existing, cancellationToken);
            if (currentRoles.Any())
            {
                await _users.RemoveFromRolesAsync(existing, currentRoles, cancellationToken);
            }
            await _users.EnsureRoleAsync(user.Role!, cancellationToken);
            await _users.AddToRoleAsync(existing, user.Role!, cancellationToken);
        }
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var entity = await _users.GetByIdAsync(id, cancellationToken);
        if (entity is null) return;
        _users.Remove(entity);
        await _users.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _users.CountAsync(cancellationToken);

    private static UserDto ToDto(ApplicationUser u) => new()
    {
        Id = u.Id,
        Email = u.Email ?? string.Empty,
        PhoneNumber = u.PhoneNumber,
        UserName = u.UserName,
        Role = u.Role.ToString()
    };

    private static ApplicationUser FromDto(UserDto d) => new()
    {
        Id = d.Id,
        Email = d.Email,
        PhoneNumber = d.PhoneNumber,
        UserName = d.UserName
    };
}
