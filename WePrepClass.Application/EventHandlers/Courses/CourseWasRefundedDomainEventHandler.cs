using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Interfaces.Emails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePrepClass.Application.Interfaces;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;

namespace WePrepClass.Application.EventHandlers.Courses;

public class CourseWasRefundedDomainEventHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<CourseWasRefundedDomainEvent>
{
    public Task Handle(CourseWasRefundedDomainEvent notification, CancellationToken cancellationToken)
    {
        var notificationObject = Notification.Create(
            "Course has been refunded",
            notification.Course.Id.Value.ToString(),
            NotificationRecipientConstants.Center,
            NotificationEventType.Course);

        notificationRepository.Insert(notificationObject);

        unitOfWork.SaveChangesAsync(cancellationToken);

        return Task.CompletedTask;
    }
}

public class MailTutorWhenCourseWasRefundedDomainEventHandler(
    ILogger<CourseWasRefundedDomainEventHandler> logger,
    IEmailSender emailSender,
    IReadDbContext readDbContext
) : INotificationHandler<CourseWasRefundedDomainEvent>
{
    public async Task Handle(CourseWasRefundedDomainEvent notification, CancellationToken cancellationToken)
    {
        var tutorInfo = await readDbContext.Users
            .Where(t => t.Id.Value == notification.approvedTeachingAssignment.TutorId.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (tutorInfo == null)
        {
            logger.LogWarning("No tutor assigned to the course {CourseId}", notification.Course.Id.Value);

            return;
        }

        await emailSender.Send(
            tutorInfo.Email,
            "Course Refunded",
            $"Your fee for the course {notification.Course.Title} has been refunded. More details, please contact the admin."
        );
    }
}