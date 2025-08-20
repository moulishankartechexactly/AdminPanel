using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Management.Admin.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IWebHostEnvironment _env;
        public ProductImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveImageAsync(IFormFile file, CancellationToken ct)
        {
            var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "products");
            Directory.CreateDirectory(uploadsRoot);
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsRoot, fileName);
            await using var stream = File.Create(fullPath);
            await file.CopyToAsync(stream, ct);
            return $"/uploads/products/{fileName}";
        }

        public void DeleteImageIfExists(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            var path = relativePath.TrimStart('/', '\\');
            var full = Path.Combine(_env.WebRootPath ?? "wwwroot", path.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(full))
            {
                try { File.Delete(full); } catch { /* ignore */ }
            }
        }
    }
}
