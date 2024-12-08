using Matt.SharedKernel.Domain.Interfaces;
using MediatR;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;

namespace WePrepClass.Application.EventHandlers.Courses;

public class TeachingRequestCreatedDomainEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<TeachingRequestCreatedDomainEvent>
{
    public Task Handle(TeachingRequestCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var notificationObject = Notification.Create(
            "Teaching Request Created",
            notification.TeachingRequest.Id.Value.ToString(),
            NotificationRecipientConstants.Center,
            NotificationEventType.TeachingRequest);

        notificationRepository.Insert(notificationObject);

        unitOfWork.SaveChangesAsync(cancellationToken);

        return Task.CompletedTask;
    }
}