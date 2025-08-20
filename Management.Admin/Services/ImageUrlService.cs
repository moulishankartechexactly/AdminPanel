using Microsoft.AspNetCore.Http;
using Model.Dtos;

namespace Management.Admin.Services;

public class ImageUrlService : IImageUrlService
{
    private readonly IHttpContextAccessor _http;
    public ImageUrlService(IHttpContextAccessor http) => _http = http;

    public void MakeAbsolute(ProductDto dto)
        => dto.ImageUrl = ToAbsolute(dto.ImageUrl);

    public string? ToAbsolute(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return path;
        if (Uri.IsWellFormedUriString(path, UriKind.Absolute)) return path;
        var req = _http.HttpContext?.Request;
        if (req == null) return path; // fallback when no request context
        var baseUrl = $"{req.Scheme}://{req.Host}{req.PathBase}";
        var rel = path.StartsWith('/') ? path : "/" + path;
        return baseUrl + rel;
    }

    public string? ToRelative(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            return url.StartsWith('/') ? url : "/" + url.TrimStart('/');
        }
        var req = _http.HttpContext?.Request;
        if (req != null)
        {
            var baseUrl = $"{req.Scheme}://{req.Host}{req.PathBase}".TrimEnd('/');
            if (url.StartsWith(baseUrl, StringComparison.OrdinalIgnoreCase))
            {
                var rel = url.Substring(baseUrl.Length);
                return string.IsNullOrEmpty(rel) ? "/" : (rel.StartsWith('/') ? rel : "/" + rel);
            }
        }
        var idx = url.IndexOf("/uploads/", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
        {
            return url.Substring(idx);
        }
        return null;
    }
}
