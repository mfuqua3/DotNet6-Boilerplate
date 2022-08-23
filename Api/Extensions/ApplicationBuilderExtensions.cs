using Api.Middleware;

namespace Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}