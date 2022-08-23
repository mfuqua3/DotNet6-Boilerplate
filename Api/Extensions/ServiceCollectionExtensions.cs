using Api.Middleware;
using Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddExceptionHandling(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();
        services.AddTransient<IConfigureOptions<ApiBehaviorOptions>, ConfigureExceptionHandlingApiBehavior>();
    }

    public static void AddOptionsConfigurations(this IServiceCollection services)
    {
        services
            .AddTransient<IConfigureOptions<ApiExplorerOptions>, ConfigureApiExplorerOptions>()
            .AddTransient<IConfigureOptions<ApiVersioningOptions>, ConfigureApiVersioningOptions>()
            .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
    }
}