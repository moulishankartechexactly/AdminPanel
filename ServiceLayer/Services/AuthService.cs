using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Entity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Interfaces;

namespace ServiceLayer.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<ApplicationUser> userManager,
                       SignInManager<ApplicationUser> signInManager,
                       IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    public async Task<(bool Succeeded, string? Error)> PasswordSignInAsync(string email, string password, bool isPersistent, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, "Invalid login attempt.");

        var result = await _signInManager.PasswordSignInAsync(user.UserName!, password, isPersistent, lockoutOnFailure: false);
        if (result.Succeeded)
            return (true, null);

        return (false, "Invalid login attempt.");
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<TokenResult?> IssueJwtAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return null;

        var pass = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!pass.Succeeded)
            return null;

        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key");
        var issuer = jwtSection.GetValue<string>("Issuer");
        var audience = jwtSection.GetValue<string>("Audience");
        var expiresMinutes = jwtSection.GetValue<int>("ExpiresMinutes");
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("JWT key not configured");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expiresMinutes > 0 ? expiresMinutes : 60);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResult(tokenString, expires);
    }
}
