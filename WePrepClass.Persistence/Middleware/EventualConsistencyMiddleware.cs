using Matt.SharedKernel.Domain.EventualConsistency;
using Matt.SharedKernel.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using WePrepClass.Persistence.EntityFrameworkCore;

namespace WePrepClass.Persistence.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate next)
{
    public const string DomainEventsKey = "DomainEventsKey";

    public async Task InvokeAsync(
        HttpContext context,
        IPublisher publisher,
        AppDbContext dbContext,
        IAppLogger<EventualConsistencyMiddleware> logger
    )
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();

        context.Response.OnCompleted(async () =>
        {
            try
            {
                if (context.Items.TryGetValue(DomainEventsKey, out var value) &&
                    value is Queue<IDomainEvent> domainEvents)
                {
                    while (domainEvents.TryDequeue(out var nextEvent))
                    {
                        await publisher.Publish(nextEvent);
                    }
                }

                await transaction.CommitAsync();
            }
            catch (EventualConsistencyException eventualConsistencyException)
            {
                // Handle eventual consistency exceptions
                logger.LogError("{Message}", eventualConsistencyException.Message);

                await transaction.RollbackAsync();
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        });

        await next(context);
    }
}