using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.Entities;

public class CourseRequest : AuditedEntity<CourseRequestId>
{
    public TutorId TutorId { get; private set; } = null!;
    public CourseId CourseId { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public RequestStatus CourseRequestStatus { get; private set; } = RequestStatus.InProgress;

    private CourseRequest()
    {
    }

    public static CourseRequest Create(TutorId tutorId, CourseId courseId) => new()
    {
        Id = CourseRequestId.Create(),
        TutorId = tutorId,
        CourseId = courseId,
        Description = "The request is in progress, please wait for the administrator's approval"
    };

    public void Cancel(string description = "The request has been cancelled")
    {
        CourseRequestStatus = RequestStatus.Denied;
        Description = description;
    }

    public void Approved()
    {
        CourseRequestStatus = RequestStatus.Approved;

        Description = "The request has been approved. Please check course's contact information as soon as possible";
    }
}