using Microsoft.AspNetCore.Builder;

namespace OnlineDemonstrator.MobileApi.CustomExceptionMiddleware.Extensions
{
    internal static class ExceptionMiddlewareExtensions
    {
        public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}