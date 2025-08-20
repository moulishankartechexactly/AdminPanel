using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Model.Dtos;
using ServiceLayer.Interfaces;
using Management.Admin.Services;

namespace Management.Admin.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductService _products;
    private readonly IProductImageService _images;
    private readonly IImageUrlService _url;
    public ProductsController(IProductService products, IProductImageService images, IImageUrlService url)
    {
        _products = products;
        _images = images;
        _url = url;
    }


    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _products.GetAllAsync(ct));

    public IActionResult Create() => View(new ProductDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductDto dto, IFormFile? imageFile, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);
        if (imageFile != null && imageFile.Length > 0)
        {
            dto.ImageUrl = await _images.SaveImageAsync(imageFile, ct);
        }
        else if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            // If user pasted an absolute URL, keep only relative path to avoid storing absolute
            dto.ImageUrl = _url.ToRelative(dto.ImageUrl);
        }
        await _products.CreateAsync(dto, ct);
        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var item = await _products.GetByIdAsync(id, ct);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductDto dto, IFormFile? imageFile, bool? removeImage, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);
        // Load existing to know current image
        var existing = await _products.GetByIdAsync(dto.Id, ct);
        if (existing == null) return NotFound();

        if (removeImage == true)
        {
            _images.DeleteImageIfExists(existing.ImageUrl);
            dto.ImageUrl = null;
        }
        else if (imageFile != null && imageFile.Length > 0)
        {
            // Replace existing file
            _images.DeleteImageIfExists(existing.ImageUrl);
            dto.ImageUrl = await _images.SaveImageAsync(imageFile, ct);
        }
        else
        {
            // Keep old path
            dto.ImageUrl = existing.ImageUrl;
        }
        await _products.UpdateAsync(dto, ct);
        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await _products.GetByIdAsync(id, ct);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var existing = await _products.GetByIdAsync(id, ct);
        if (existing != null)
        {
            _images.DeleteImageIfExists(existing.ImageUrl);
        }
        await _products.DeleteAsync(id, ct);
        TempData["Success"] = "Product deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var item = await _products.GetByIdAsync(id, ct);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveImage(int id, CancellationToken ct)
    {
        var existing = await _products.GetByIdAsync(id, ct);
        if (existing == null) return NotFound();
        _images.DeleteImageIfExists(existing.ImageUrl);
        existing.ImageUrl = null;
        await _products.UpdateAsync(existing, ct);
        TempData["Success"] = "Product image removed.";
        return RedirectToAction(nameof(Edit), new { id });
    }

    // URL normalization moved to IImageUrlService
}
