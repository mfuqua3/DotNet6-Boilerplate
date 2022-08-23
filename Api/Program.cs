using Data.Components;
using Serilog;

namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        // await host.Services
        //     .CreateScope()
        //     .ServiceProvider
        //     .GetRequiredService<DbContext>()
        //     .Database
        //     .EnsureCreatedAsync();
        await host.RunAsync();
    }
    
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((ctx, lc) =>
            {
                lc.ReadFrom.Configuration(ctx.Configuration);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}