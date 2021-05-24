using System;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            
            // Create a database and apply migrations on startup
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                // Get services
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                
                try
                {
                    var context = services.GetRequiredService<StoreContext>();
                    // Create a database (if doesn't exist) and apply any pending migrations
                    await context.Database.MigrateAsync();
                    
                    // Seed the database from the JSON files
                    await StoreContextSeed.SeedAsync(context, loggerFactory);
                }
                catch (Exception e)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(e, "An error occured during migration");
                }
            }
            // Start the app
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}