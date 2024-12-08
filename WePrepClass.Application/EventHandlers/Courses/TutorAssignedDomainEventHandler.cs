using Matt.SharedKernel.Domain.Interfaces.Emails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePrepClass.Application.Interfaces;
using WePrepClass.Domain.WePrepClassAggregates.Courses;

namespace WePrepClass.Application.EventHandlers.Courses;

public class TutorAssignedDomainEventHandler(
    IReadDbContext dbContext,
    ILogger<TutorAssignedDomainEventHandler> logger,
    IEmailSender emailSender
) : INotificationHandler<TutorAssignedDomainEvent>
{
    public async Task Handle(TutorAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        var teachingAssignment = notification.Course.GetApprovedTeachingAssignment();

        if (teachingAssignment is null)
        {
            logger.LogWarning("No active teaching assignment found for course {CourseId}",
                notification.Course.Id.Value);

            return;
        }

        var tutorAccountInf = await dbContext.Users.FirstOrDefaultAsync(t => t.Id == teachingAssignment.TutorId,
            cancellationToken: cancellationToken);

        if (tutorAccountInf is null)
        {
            logger.LogWarning("No tutor found for teaching assignment {TeachingAssignmentId}",
                teachingAssignment.Id.Value);

            return;
        }

        await emailSender.Send(
            tutorAccountInf.Email,
            "Tutor Assignment",
            $"You have been assigned to tutor the course {notification.Course.Title}. Please login to your account to view the course details."
        );
    }
}