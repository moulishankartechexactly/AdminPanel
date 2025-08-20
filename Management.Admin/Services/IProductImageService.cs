using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Management.Admin.Services
{
    public interface IProductImageService
    {
        Task<string> SaveImageAsync(IFormFile file, CancellationToken ct);
        void DeleteImageIfExists(string? relativePath);
    }
}
