using System.Reflection;
using Data.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DependencyInjection;

public static class CoreRegistrar
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<DbContext>();
        services.AddAutoMapper(cfg => { cfg.AddMaps(Assembly.GetExecutingAssembly()); });
        return services;
    }
}