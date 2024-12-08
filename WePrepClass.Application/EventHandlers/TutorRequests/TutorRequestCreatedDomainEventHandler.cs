using Matt.SharedKernel.Domain.Interfaces;
using MediatR;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;
using WePrepClass.Domain.WePrepClassAggregates.TutoringRequests;

namespace WePrepClass.Application.EventHandlers.TutorRequests;

public class TutorRequestCreatedDomainEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<TutorRequestCreatedDomainEvent>
{
    public Task Handle(TutorRequestCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var notificationObject = Notification.Create(
            "New tutoring request was created",
            notification.TutoringRequest.Id.Value.ToString(),
            NotificationRecipientConstants.Center,
            NotificationEventType.TutoringRequest);

        notificationRepository.Insert(notificationObject);

        unitOfWork.SaveChangesAsync(cancellationToken);

        return Task.CompletedTask;
    }
}
