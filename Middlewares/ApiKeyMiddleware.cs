using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace invoice.Middlewares
{
    public class ApiKeyMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyMiddleware> _logger;

        public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }
        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Request.Headers.TryGetValue("APIKEY", out var extractedApiKey))
            {
                _logger.LogWarning("API Key missing from request {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key missing");
                return;
            }

            var key = extractedApiKey.ToString();

            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("Invalid API Key: empty value for request {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.ApiKey == key && u.IsDeleted == false);

            if (user == null)
            {
                _logger.LogWarning("Invalid API Key attempt: {ApiKey}", key);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }
            _logger.LogInformation("API Key login successful for UserId {UserId}", user.Id);
            context.Items["ExternalUserId"] = user.Id;

            await _next(context);
        }
    }

    }

