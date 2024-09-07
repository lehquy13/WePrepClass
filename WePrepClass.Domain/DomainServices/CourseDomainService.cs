using Matt.ResultObject;
using WePrepClass.Domain.Commons;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.TeachingRequests;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.DomainServices;

public interface ICourseDomainService
{
    Task<Result> CreateTeachingRequest(
        CourseId courseId,
        TutorId tutorId,
        CancellationToken cancellationToken = default);

    Task<Result> AssignTutorToCourse(
        CourseId courseId,
        TutorId tutorId,
        CancellationToken cancellationToken = default);

    Task<Result> DissociateTutor(
        CourseId courseId,
        string detailMessage = "",
        CancellationToken cancellationToken = default);
}

public class CourseDomainService(
    ICourseRepository courseRepository,
    ITeachingRequestRepository teachingRequestRepository,
    ITutorRepository tutorRepository
) : DomainServiceBase, ICourseDomainService
{
    public async Task<Result> CreateTeachingRequest(
        CourseId courseId,
        TutorId tutorId,
        CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetById(courseId, cancellationToken);

        if (course is null) return DomainErrors.Courses.NotFound;

        if (course.Status is not CourseStatus.Available) return DomainErrors.Courses.Unavailable;

        var tutor = await tutorRepository.GetById(tutorId, cancellationToken);

        if (tutor is null) return DomainErrors.Tutors.NotFound;

        var teachingRequest =
            await teachingRequestRepository.GetByCourseIdAndTutorId(course.Id, tutor.Id, cancellationToken);

        if (teachingRequest is not null) return DomainErrors.Courses.TeachingRequestAlreadyExist;

        var newTeachingRequest = TeachingRequest.Create(tutorId, courseId);

        await teachingRequestRepository.Insert(newTeachingRequest);

        return Result.Success();
    }

    public async Task<Result> AssignTutorToCourse(
        CourseId courseId,
        TutorId tutorId,
        CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetById(courseId, cancellationToken);

        if (course is null) return DomainErrors.Courses.NotFound;

        var tutor = await tutorRepository.GetById(tutorId, cancellationToken);

        if (tutor is null) return DomainErrors.Tutors.NotFound;

        var result = course.AssignTutor(tutor.Id);

        if (result.IsFailed) return result;

        var teachingRequests =
            await teachingRequestRepository.GetTeachingRequestsByCourseId(courseId, cancellationToken);

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

    public async Task<Result> DissociateTutor(
        CourseId courseId,
        string detailMessage = "",
        CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetById(courseId, cancellationToken);

        if (course is null) return DomainErrors.Courses.NotFound;

        var tutorId = course.TutorId;

        var dissociateTutor = course.DissociateTutor(detailMessage);

        if (dissociateTutor.IsFailed) return dissociateTutor;

        // null-forgiving operator is used because the tutorId is not null after course's dissociation execution
        var teachingRequest =
            await teachingRequestRepository.GetByCourseIdAndTutorId(courseId, tutorId!, cancellationToken);

        // Check if there is a request approved before, then cancel it
        teachingRequest?.Cancel();

        return Result.Success();
    }
}