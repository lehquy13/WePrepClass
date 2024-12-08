using Matt.SharedKernel.Domain.Interfaces;
using MediatR;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;

namespace WePrepClass.Application.EventHandlers.Courses;

public class CourseCreatedCourseDomainEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<CourseCreatedCourseDomainEvent>
{
    public Task Handle(CourseCreatedCourseDomainEvent notification, CancellationToken cancellationToken)
    {
        var notificationObject = Notification.Create(
            "New course had been requested",
            notification.Course.Id.Value.ToString(),
            NotificationRecipientConstants.Center,
            NotificationEventType.Course);

        notificationRepository.Insert(notificationObject);

        unitOfWork.SaveChangesAsync(cancellationToken);

        return Task.CompletedTask;
    }
}