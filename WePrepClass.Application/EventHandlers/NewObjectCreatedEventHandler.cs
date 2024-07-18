using Matt.SharedKernel.Domain;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Interfaces.Repositories;
using MediatR;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;

namespace WePrepClass.Application.EventHandlers;

public class NewObjectCreatedEventHandler(
    IRepository<Notification, int> notificationRepository,
    IAppLogger<NewObjectCreatedEventHandler> logger,
    IUnitOfWork unitOfWork
) : INotificationHandler<NewObjectCreatedEvent>
{
    public async Task Handle(NewObjectCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new notification...");

        var entityToCreate = Notification.Create(
            notification.Message,
            notification.ObjectId);

        await notificationRepository.InsertAsync(entityToCreate, cancellationToken);

        if (await unitOfWork.SaveChangesAsync(cancellationToken) <= 0)
        {
            logger.LogError("Fail to add new notification while handling {0}", notification.GetType().Name);
            return;
        }

        logger.LogInformation("Created new notification {0}", notification.GetType().Name);
    }
}