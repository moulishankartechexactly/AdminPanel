using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos;
using ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Management.Admin.Controllers.Api;

[ApiController]
[Route("api/users")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public class ApiUsersController : ControllerBase
{
    private readonly IUserService _users;
    public ApiUsersController(IUserService users) => _users = users;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken ct)
        => Ok(await _users.GetAllAsync(ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(string id, CancellationToken ct)
    {
        var item = await _users.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(UserDto dto, CancellationToken ct)
    {
        var created = await _users.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UserDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest();
        await _users.UpdateAsync(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        await _users.DeleteAsync(id, ct);
        return NoContent();
    }
}
