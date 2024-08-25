using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.TeachingRequests.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.TeachingRequests;

public class TeachingRequest : AuditedAggregateRoot<TeachingRequestId>
{
    public TutorId TutorId { get; private set; } = null!;
    public CourseId CourseId { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public RequestStatus TeachingRequestStatus { get; private set; } = RequestStatus.InProgress;

    private TeachingRequest()
    {
    }

    public static TeachingRequest Create(TutorId tutorId, CourseId courseId)
    {
        var teachingRequest = new TeachingRequest
        {
            Id = TeachingRequestId.Create(),
            TutorId = tutorId,
            CourseId = courseId,
            Description = "The request is in progress, please wait for the administrator's approval"
        };

        teachingRequest.DomainEvents.Add(new TeachingRequestCreatedDomainEvent(teachingRequest));

        return teachingRequest;
    }

    public void Cancel(string description = "The request has been cancelled")
    {
        TeachingRequestStatus = RequestStatus.Denied;
        Description = description;
    }

    public void Approved()
    {
        TeachingRequestStatus = RequestStatus.Approved;

        Description = "The request has been approved. Please check course's contact information as soon as possible";
    }
}

// ReSharper disable NotAccessedPositionalProperty.Global
public record TeachingRequestCreatedDomainEvent(TeachingRequest TeachingRequest) : IDomainEvent;