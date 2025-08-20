using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos;
using ServiceLayer.Interfaces;

namespace Management.Admin.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly IUserService _users;
    public UsersController(IUserService users) => _users = users;

    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _users.GetAllAsync(ct));

    public async Task<IActionResult> Details(string id, CancellationToken ct)
    {
        var item = await _users.GetByIdAsync(id, ct);
        if (item == null) return NotFound();
        return View(item);
    }

    public IActionResult Create() => View(new UserDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);
        await _users.CreateAsync(dto, ct);
        TempData["Success"] = "User created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id, CancellationToken ct)
    {
        var item = await _users.GetByIdAsync(id, ct);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);
        await _users.UpdateAsync(dto, ct);
        TempData["Success"] = "User updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var item = await _users.GetByIdAsync(id, ct);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id, CancellationToken ct)
    {
        await _users.DeleteAsync(id, ct);
        TempData["Success"] = "User deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
