using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos;
using ServiceLayer.Interfaces;

namespace Management.Admin.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class ApiAuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public ApiAuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and Password are required.");

        var token = await _authService.IssueJwtAsync(request.Email, request.Password, ct);
        if (token == null)
            return Unauthorized();

        return Ok(new { access_token = token.AccessToken, expires_at_utc = token.ExpiresAtUtc });
    }
}
