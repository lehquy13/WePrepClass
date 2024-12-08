using Matt.SharedKernel.Domain.Interfaces;
using MediatR;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;

namespace WePrepClass.Application.EventHandlers.Tutors;

public class TutorUpdatedDomainEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<TutorUpdatedDomainEvent>
{
    public Task Handle(TutorUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var notificationObject = Notification.Create(
            "Tutor profile was updated",
            notification.Tutor.Id.Value.ToString(),
            NotificationRecipientConstants.Center,
            NotificationEventType.Tutor);

        notificationRepository.Insert(notificationObject);

        unitOfWork.SaveChangesAsync(cancellationToken);

        return Task.CompletedTask;
    }
}