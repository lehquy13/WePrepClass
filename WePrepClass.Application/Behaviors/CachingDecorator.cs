using System.Text.Json;
using LazyCache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace WePrepClass.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse>(
    IAppCache cache,
    ILogger<CachingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, new()
{
    public Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var defaultRequestKey = GenerateCacheKey(new TRequest());
        var key = GenerateCacheKey(request);

        logger.LogInformation("Generated key: {Key}", key);

        if (defaultRequestKey.Equals(key))
        {
            return cache.GetOrAddAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await next();
            });
        }

        return next();
    }

    private static string GenerateCacheKey(TRequest request)
    {
        // Implement your logic to generate a unique cache key based on the request
        // You can concatenate request properties or serialize the request object, depending on your requirements
        // Return a string representing the cache key

        return request.GetType() + JsonSerializer.Serialize(request);
    }
}