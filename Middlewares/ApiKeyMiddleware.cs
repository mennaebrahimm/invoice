using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace invoice.Middlewares
{
    public class ApiKeyMiddleware
    {

        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;

        }
        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Request.Headers.TryGetValue("APIKEY", out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key missing");
                return;
            }

            var key = extractedApiKey.ToString();

            if (string.IsNullOrWhiteSpace(key))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.ApiKey == key && u.IsDeleted == false);

            if (user == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            context.Items["ExternalUserId"] = user.Id;

            await _next(context);
        }
    }

    }

