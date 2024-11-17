using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.Entities;

public class TeachingAssignment : FullAuditedEntity<TeachingAssignmentId>
{
    public TutorId TutorId { get; private set; } = null!;
    public CourseId CourseId { get; private set; } = null!;

    public TeachingAssignmentStatus TeachingAssignmentStatus { get; private set; } = TeachingAssignmentStatus.Assigned;

    private TeachingAssignment()
    {
    }

    public static TeachingAssignment Create(TutorId tutorId, CourseId courseId)
    {
        var tutorRequest = new TeachingAssignment
        {
            Id = TeachingAssignmentId.Create(),
            TutorId = tutorId,
            CourseId = courseId
        };

        return tutorRequest;
    }

    public void Dissociate()
    {
        TeachingAssignmentStatus = TeachingAssignmentStatus.Dissociated;
    }
}