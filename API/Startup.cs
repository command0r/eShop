using API.Extensions;
using API.Helpers;
using API.Middleware;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding AutoMapper
            services.AddAutoMapper(typeof(MappingProfiles));
            
            // Other services
            services.AddControllers();
            services.AddDbContext<StoreContext>(x =>
                x.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));

            // Adding Redis configuration
            services.AddSingleton<IConnectionMultiplexer>(c =>
            {
                var config = ConfigurationOptions.Parse(_configuration.
                        GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(config);
            }); 
            
            services.AddApplicationServices();
            services.AddSwaggerDocumentation();

            // Configure CORS support
            services.AddCors(opt =>
                opt.AddPolicy("CorsPolicy",
                    policy =>
                    {
                        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                    })
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // A custom exception handling middleware (has to be at the top)
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwaggerDocumentation();

            // Request goes to an API and hits the middleware (the order here is important)
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}