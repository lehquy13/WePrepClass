using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Administrator.Courses.Commands;

public record AssignTutorCommand(Guid CourseId, Guid TutorId) : ICommandRequest;

public class AssignTutorCommandHandler(
    ICourseRepository courseRepository,
    ITutorRepository tutorRepository,
    IUnitOfWork unitOfWork,
    IAppLogger<AssignTutorCommandHandler> logger
) : CommandHandlerBase<AssignTutorCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(AssignTutorCommand command, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetById(CourseId.Create(command.CourseId), cancellationToken);

        if (course is null) return DomainErrors.Courses.NotFound;

        var tutor = await tutorRepository.GetById(TutorId.Create(command.TutorId), cancellationToken);

        if (tutor is null) return DomainErrors.Tutors.NotFound;

        var result = course.AssignTutor(tutor.Id);

        return result.IsFailed ? result : Result.Success();
    }
}