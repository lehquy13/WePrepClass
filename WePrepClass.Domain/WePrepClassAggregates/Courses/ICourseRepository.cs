using Matt.SharedKernel.Domain.Interfaces.Repositories;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses;

public interface ICourseRepository : IRepository
{
    Task<List<Course>> GetLearningCoursesByUserId(UserId learnerId);
    Task<bool> IsCoursesRequestedByTutor(UserId tutorId, CourseId classId);
    Task<Course?> GetCourseByCourseRequestId(TeachingRequestId teachingRequestId, CancellationToken cancellationToken);
    Task<List<Course>> GetAllTutorRelatedCourses(TutorId tutorId);
}