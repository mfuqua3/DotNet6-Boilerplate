using Api.Extensions;
using Core.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services.AddOptionsConfigurations();
        services.AddHealthChecks();
        services.AddControllers();
        services.AddSwaggerGen();
        services.AddApiVersioning();
        services.AddVersionedApiExplorer();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        services.AddAuthorization();
        services.AddExceptionHandling();
        services.AddCore(_configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandling();
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapControllers();
        });
    }
}