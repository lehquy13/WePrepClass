using Matt.SharedKernel.Domain.Interfaces.Emails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePrepClass.Application.Interfaces;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;

namespace WePrepClass.Application.EventHandlers.Courses;

public class CourseRequirementUpdatedDomainEventHandler(
    ILogger<CourseRequirementUpdatedDomainEventHandler> logger,
    IReadDbContext dbContext,
    IEmailSender emailSender
) : INotificationHandler<CourseRequirementUpdatedDomainEvent>
{
    public async Task Handle(CourseRequirementUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        foreach (var ta in notification.Course.TeachingRequests.Where(x =>
                     x.TeachingRequestStatus != RequestStatus.Denied))
        {
            var tutorAccountInf = await dbContext.Users.FirstOrDefaultAsync(t => t.Id.Value == ta.TutorId.Value,
                cancellationToken: cancellationToken);

            if (tutorAccountInf is null)
            {
                logger.LogWarning(
                    "Can not found tutor of teaching assignment {TeachingAssignmentId}, tutorId: {TutorId}",
                    ta.Id.Value, ta.TutorId.Value);

                continue;
            }

            await emailSender.Send(
                tutorAccountInf.Email,
                "Course requirement updated",
                $"Course requirement of {notification.Course.Title} has been updated. Please check the course details."
            );
        }
    }
}