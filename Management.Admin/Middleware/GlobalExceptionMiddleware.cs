using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Management.Admin.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing request {Path}", context.Request.Path);

            if (IsApiRequest(context.Request))
            {
                await WriteProblemDetailsAsync(context, ex);
            }
            else
            {
                // For MVC requests, redirect to generic error page
                context.Response.Clear();
                context.Response.Redirect("/Home/Error");
            }
        }
    }

    private static bool IsApiRequest(HttpRequest request)
    {
        // Heuristic: path starts with /api or Accept prefers JSON
        if (request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            return true;

        var accept = request.Headers["Accept"].ToString();
        return accept.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
               accept.Contains("text/json", StringComparison.OrdinalIgnoreCase) ||
               request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
    {
        var status = (int)HttpStatusCode.InternalServerError;

        var problem = new ProblemDetails
        {
            Title = "An unexpected error occurred",
            Status = status,
            Detail = ex.Message,
            Instance = context.TraceIdentifier,
            Type = "https://httpstatuses.com/500"
        };

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<Management.Admin.Middleware.GlobalExceptionMiddleware>();
}
