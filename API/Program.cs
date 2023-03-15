using API.Extensions;
using API.Middleware;
using Core.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

// Add services to the container
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure the http request pipeline (comes from the legacy 'Startup.cs')
// A custom exception handling middleware (has to be at the top)
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwaggerDocumentation();

// Request goes to an API and hits the middleware (the order here is important)
app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseHttpsRedirection();

app.UseRouting();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Content")
    ), RequestPath = "/content"
});

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();

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

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var identityContext = services.GetRequiredService<AppIdentityDbContext>();
    await identityContext.Database.MigrateAsync();
    await AppIdentityContextSeed.SeedUsersAsync(userManager);
}
catch (Exception e)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(e, "An error occured during migration");
}

// Start the app
await app.RunAsync();