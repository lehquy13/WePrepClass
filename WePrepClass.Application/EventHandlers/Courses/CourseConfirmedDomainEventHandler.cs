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

public class CourseConfirmedDomainEventHandler(
    ILogger<CourseConfirmedDomainEventHandler> logger,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<CourseConfirmedDomainEvent>
{
    public Task Handle(CourseConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        var assignedTutor = notification.Course.GetApprovedTeachingAssignment()?.TutorId;

        if (assignedTutor == null)
        {
            logger.LogWarning("No tutor assigned to the course {CourseId}", notification.Course.Id.Value);

            return Task.CompletedTask;
        }

        var notificationObject = Notification.Create(
            $"Course {notification.Course.Title} has been ready for teaching",
            notification.Course.Id.Value.ToString(),
            assignedTutor.Value,
            NotificationEventType.Course);

        notificationRepository.Insert(notificationObject);

        unitOfWork.SaveChangesAsync(cancellationToken);

        return Task.CompletedTask;
    }
}

public class MailToTutorWhenCourseConfirmedDomainEventHandler(
    ILogger<MailToTutorWhenCourseConfirmedDomainEventHandler> logger,
    IReadDbContext readDbContext,
    IEmailSender emailSender
) : INotificationHandler<CourseConfirmedDomainEvent>
{
    public async Task Handle(CourseConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        var assignedTutor = notification.Course.GetApprovedTeachingAssignment()?.TutorId;

        if (assignedTutor == null)
        {
            logger.LogWarning("No tutor assigned to the course {CourseId}", notification.Course.Id.Value);

            return;
        }

        var tutorInfo = await readDbContext.Users
            .Where(t => t.Id.Value == assignedTutor.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (tutorInfo == null)
        {
            logger.LogWarning("Assigned tutor info - id: {TutorId} - is not found", assignedTutor.Value);

            return;
        }

        await emailSender.Send(
            tutorInfo.Email,
            "Course Confirmed",
            $"Course {notification.Course.Title} has been confirmed and ready for teaching"
        );

        return;
    }
}