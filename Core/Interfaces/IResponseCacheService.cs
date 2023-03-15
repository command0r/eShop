namespace Core.Interfaces;

public interface IResponseCacheService
{
    // Handling caching with Redis
    Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
    Task<string> GetCachedResponseAsync(string cacheKey);
}