
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Results;
using WePrepClass.Domain;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Courses.Commands;

public record CreateCourseRequestCommand(Guid CourseId) : ICommandRequest, IAuthorizationRequired;

public class CreateCourseRequestCommandHandler(
    ICourseRepository courseRepository,
    ITutorRepository tutorRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork
) : CommandHandlerBase<CreateCourseRequestCommand>(unitOfWork)
{
    public override async Task<Result> Handle(CreateCourseRequestCommand command, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetById(
            CourseId.Create(command.CourseId), cancellationToken);

        if (course is null) return DomainErrors.Courses.NotFound;

        if (course.Status is not CourseStatus.Available) return DomainErrors.Courses.Unavailable;

        var tutor = await tutorRepository.GetById(TutorId.Create(currentUserService.UserId), cancellationToken);

        if (tutor is null) return DomainErrors.Tutors.NotFound;

        var result = course.AddTeachingRequest(tutor.Id);

        if (result.IsFailed) return result;

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}