using API.Errors;
using API.Helpers;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace API.Extensions
{
    // Extension class to keep Startup.cs neat and tidy
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Adding AutoMapper
            services.AddAutoMapper(typeof(MappingProfiles));

            // Postgres server Db context
            services.AddDbContext<StoreContext>(x =>
                x.UseNpgsql(config.GetConnectionString("DefaultConnection")));

            // Adding Redis configuration
            services.AddSingleton<IConnectionMultiplexer>(c =>
            {
                var configuration = ConfigurationOptions.Parse(config.
                    GetConnectionString("Redis")!, true);
                return ConnectionMultiplexer.Connect(configuration);
            });

            // Configure CORS support
            services.AddCors(opt =>
                opt.AddPolicy("CorsPolicy",
                    policy =>
                    {
                        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                    })
            );
            
            // Adding repositories
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}