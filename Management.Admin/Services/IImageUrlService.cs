using Microsoft.AspNetCore.Http;
using Model.Dtos;

namespace Management.Admin.Services;

public interface IImageUrlService
{
    string? ToRelative(string? url);
    string? ToAbsolute(string? path);
    void MakeAbsolute(ProductDto dto);
}
