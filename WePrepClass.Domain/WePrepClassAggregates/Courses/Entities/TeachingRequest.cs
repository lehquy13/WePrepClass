using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.Entities;

public class TeachingRequest : AuditedEntity<TeachingRequestId>
{
    public TutorId TutorId { get; private set; } = null!;
    public CourseId CourseId { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public RequestStatus TeachingRequestStatus { get; private set; } = RequestStatus.InProgress;

    private TeachingRequest()
    {
    }

    public static TeachingRequest Create(TutorId tutorId, CourseId courseId) => new()
    {
        Id = TeachingRequestId.Create(),
        TutorId = tutorId,
        CourseId = courseId,
        Description = "The request is in progress, please wait for the administrator's approval"
    };

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