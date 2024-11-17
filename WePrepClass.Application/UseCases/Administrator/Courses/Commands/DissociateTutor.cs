using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

namespace WePrepClass.Application.UseCases.Administrator.Courses.Commands;

public record DissociateTutorCommand(Guid CourseId, string DetailMessage) : ICommandRequest;

public class DissociateTutorCommandHandler(
    ICourseRepository courseRepository,
    IUnitOfWork unitOfWork,
    IAppLogger<DissociateTutorCommandHandler> logger
) : CommandHandlerBase<DissociateTutorCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(DissociateTutorCommand command, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetById(CourseId.Create(command.CourseId), cancellationToken);

        if (course is null) return DomainErrors.Courses.NotFound;

        var result = course.DissociateTutor(command.DetailMessage);

        return result.IsFailed ? result : Result.Success();
    }
}