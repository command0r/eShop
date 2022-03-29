using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    // Filters allow to run code before or after specific stages in a request processing pipeline
    // Here, we need a code to run immediately before the Action method is called (in a controller) and after
    // the method has been executed. The code will look into the cache first, and if the item is not there, it'll be
    // cached after the call is made, so the next method call takes a value from the cache
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get reference to cache service (from Infrastructure.Services)
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            
            // Generate a key (to identify a piece of data that will be retrieved)
            // We need the same key for the same response (build a cacheKey basing on the query string parameters)
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    // Instead of letting the request to get to a controller, we attempt to retrieve it from
                    // memory first (Redis cache) and return straight to a client
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;
                
                return;
            }

            // Move to controller
            var executedContext = await next();
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                // Cache the response into Redis (so, the next time it's available for retrieval)
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value,
                    TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}