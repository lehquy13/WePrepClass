using Matt.ResultObject;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.TeachingRequests;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.DomainServices;

public interface ICourseDomainService
{
    Task<Result> AddTeachingRequestToCourse(CourseId courseId, TutorId tutorId);

    Task<Result> AssignTutorToCourse(CourseId courseId, TutorId tutorId);

    Task<Result> DissociateTutorFromCourse(CourseId courseId, TutorId tutorId, string detailMessage = "");
}

public class CourseDomainService(
    ICourseRepository courseRepository,
    ITeachingRequestRepository teachingRequestRepository
) : ICourseDomainService
{
    public async Task<Result> AddTeachingRequestToCourse(CourseId courseId, TutorId tutorId)
    {
        var course = await courseRepository.GetById(courseId);

        if (course is null) return DomainErrors.Courses.NotFound;

        if (course.Status is not CourseStatus.Available) return DomainErrors.Courses.Unavailable;

        var teachingRequest = await teachingRequestRepository.GetByCourseId(courseId);

        if (teachingRequest is not null) return DomainErrors.Courses.TeachingRequestAlreadyExist;

        var newTeachingRequest = TeachingRequest.Create(tutorId, courseId);

        await teachingRequestRepository.Insert(newTeachingRequest);

        return Result.Success();
    }

    public async Task<Result> AssignTutorToCourse(CourseId courseId, TutorId tutorId)
    {
        var course = await courseRepository.GetById(courseId);

        if (course is null) return DomainErrors.Courses.NotFound;

        if (course.LearnerDetail.LearnerId?.Value == tutorId.Value)
            return DomainErrors.Courses.TutorAndLearnerShouldNotBeTheSame;

        course.AssignTutor(tutorId);

        var teachingRequests = await teachingRequestRepository.GetTeachingRequestsByCourseId(courseId);

        // Check if there is a request exist before, then approve it and cancel the rest
        foreach (var courseRequest in teachingRequests)
        {
            if (courseRequest.TutorId == tutorId)
            {
                courseRequest.Approved();
            }
            else
            {
                courseRequest.Cancel();
            }
        }

        return Result.Success();
    }

    public async Task<Result> DissociateTutorFromCourse(CourseId courseId, TutorId tutorId, string detailMessage = "")
    {
        var course = await courseRepository.GetById(courseId);

        if (course is null) return DomainErrors.Courses.NotFound;

        var dissociateTutor = course.DissociateTutor();

        if (dissociateTutor.IsFailed) return dissociateTutor;

        var teachingRequests = await teachingRequestRepository.GetTeachingRequestsByCourseId(courseId);

        // Check if there is a request approved before, then cancel it
        teachingRequests.FirstOrDefault(x => x.TutorId == tutorId)?.Cancel(detailMessage);

        return Result.Success();
    }
}