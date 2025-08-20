using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos;
using ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Management.Admin.Services;

namespace Management.Admin.Controllers.Api;

[ApiController]
[Route("api/products")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ApiProductsController : ControllerBase
{
    private readonly IProductService _products;
    private readonly IProductImageService _images;
    private readonly IImageUrlService _url;
    public ApiProductsController(IProductService products, IProductImageService images, IImageUrlService url)
    {
        _products = products;
        _images = images;
        _url = url;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken ct)
    {
        var items = await _products.GetAllAsync(ct);
        foreach (var p in items) _url.MakeAbsolute(p);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct)
    {
        var item = await _products.GetByIdAsync(id, ct);
        if (item is null) return NotFound();
        _url.MakeAbsolute(item);
        return Ok(item);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProductDto>> Create([FromForm] ProductDto dto, IFormFile? imageFile, CancellationToken ct)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            dto.ImageUrl = await _images.SaveImageAsync(imageFile, ct);
        }
        else if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            dto.ImageUrl = _url.ToRelative(dto.ImageUrl);
        }
        var created = await _products.CreateAsync(dto, ct);
        _url.MakeAbsolute(created);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(int id, [FromForm] ProductDto dto, IFormFile? imageFile, bool? removeImage, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest();
        var existing = await _products.GetByIdAsync(id, ct);
        if (existing == null) return NotFound();

        if (removeImage == true)
        {
            _images.DeleteImageIfExists(existing.ImageUrl);
            dto.ImageUrl = null;
        }
        else if (imageFile != null && imageFile.Length > 0)
        {
            _images.DeleteImageIfExists(existing.ImageUrl);
            dto.ImageUrl = await _images.SaveImageAsync(imageFile, ct);
        }
        else
        {
            dto.ImageUrl = existing.ImageUrl;
        }

        await _products.UpdateAsync(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var existing = await _products.GetByIdAsync(id, ct);
        if (existing != null)
        {
            _images.DeleteImageIfExists(existing.ImageUrl);
        }
        await _products.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/remove-image")]
    public async Task<IActionResult> RemoveImage(int id, CancellationToken ct)
    {
        var existing = await _products.GetByIdAsync(id, ct);
        if (existing == null) return NotFound();
        _images.DeleteImageIfExists(existing.ImageUrl);
        existing.ImageUrl = null;
        await _products.UpdateAsync(existing, ct);
        return NoContent();
    }

    // URL conversion moved to IImageUrlService
}
