using Matt.SharedKernel.Domain.Interfaces.Repositories;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.TeachingRequests;

public interface ITeachingRequestRepository : IRepository
{
    Task<TeachingRequest?> GetByCourseId(CourseId courseId);
    Task<List<TeachingRequest>> GetTeachingRequestsByCourseId(CourseId courseId);
    Task Insert(TeachingRequest teachingRequest);
}