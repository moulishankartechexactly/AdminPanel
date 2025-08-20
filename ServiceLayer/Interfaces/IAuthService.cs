using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces;

public record TokenResult(string AccessToken, DateTime ExpiresAtUtc);

public interface IAuthService
{
    Task<(bool Succeeded, string? Error)> PasswordSignInAsync(string email, string password, bool isPersistent, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
    Task<TokenResult?> IssueJwtAsync(string email, string password, CancellationToken cancellationToken = default);
}
