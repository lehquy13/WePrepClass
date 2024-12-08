using Matt.SharedKernel.Domain.Interfaces;
using MediatR;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;

namespace WePrepClass.Application.EventHandlers.Tutors;

public class VerificationChangeCreatedDomainEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<VerificationChangeCreatedDomainEvent>
{
    public Task Handle(VerificationChangeCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var notificationObject = Notification.Create(
            "A verification change request was created",
            notification.Tutor.Id.Value.ToString(),
            NotificationRecipientConstants.Center,
            NotificationEventType.Tutor);

        notificationRepository.Insert(notificationObject);

        unitOfWork.SaveChangesAsync(cancellationToken);

        return Task.CompletedTask;
    }
}