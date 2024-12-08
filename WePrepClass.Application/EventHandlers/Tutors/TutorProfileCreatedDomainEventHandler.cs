using Matt.SharedKernel.Domain.Interfaces;
using MediatR;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;

namespace WePrepClass.Application.EventHandlers.Tutors;

public class TutorProfileCreatedDomainEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<TutorProfileCreatedDomainEvent>
{
    public Task Handle(TutorProfileCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var notificationObject = Notification.Create(
            "New tutor profile was created",
            notification.Tutor.Id.Value.ToString(),
            NotificationRecipientConstants.Center,
            NotificationEventType.Tutor);

        notificationRepository.Insert(notificationObject);

        unitOfWork.SaveChangesAsync(cancellationToken);

        return Task.CompletedTask;
    }
}