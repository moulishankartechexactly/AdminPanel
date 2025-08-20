using Model.Dtos;
using System.Threading;

namespace ServiceLayer.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(UserDto user, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserDto user, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
