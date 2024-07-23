using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Interfaces.Repositories;
using MediatR;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;

namespace WePrepClass.Application.EventHandlers;

public record VerificationChangeRequestedDomainEvent(Tutor Tutor, int ChangeVerificationRequest) : IDomainEvent;

public class VerificationChangeRequestedDomainEventHandler(
    IRepository<Notification, int> notificationRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<VerificationChangeRequestedDomainEvent>
{
    public Task Handle(VerificationChangeRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        var message =
            $"Tutor {notification.Tutor.Id.Value} has created a change verification with {notification.ChangeVerificationRequest} changes";

        notificationRepository.InsertAsync(Notification.Create(
            message,
            notification.Tutor.Id.Value.ToString(),
            NotificationEventType.Tutor
        ), cancellationToken);

        unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Task.CompletedTask;
    }
}