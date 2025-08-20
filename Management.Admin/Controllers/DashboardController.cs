using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Management.Admin.Models;
using ServiceLayer.Interfaces;

namespace Management.Admin.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IUserService _users;
    private readonly IProductService _products;

    public DashboardController(IUserService users, IProductService products)
    {
        _users = users;
        _products = products;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = new DashboardViewModel
        {
            TotalUsers = await _users.CountAsync(ct),
            TotalProducts = await _products.CountAsync(ct),
            LowStockProducts = await _products.CountLowStockAsync(10, ct),
            ProductsPerCategory = await _products.GetCountsByCategoryAsync(ct)
        };
        return View(vm);
    }
}
