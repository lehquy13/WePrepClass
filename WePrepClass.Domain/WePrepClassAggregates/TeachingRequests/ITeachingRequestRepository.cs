using Matt.SharedKernel.Domain.Interfaces.Repositories;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.TeachingRequests;

public interface ITeachingRequestRepository : IRepository
{
    Task<TeachingRequest?> GetByCourseIdAndTutorId(CourseId courseId, TutorId tutorId,
        CancellationToken cancellationToken = default);

    Task<List<TeachingRequest>> GetTeachingRequestsByCourseId(CourseId courseId,
        CancellationToken cancellationToken = default);

    Task Insert(TeachingRequest teachingRequest);
}