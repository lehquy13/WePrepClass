using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Courses.Commands;

public record ReviewCourseCommand(Guid CourseId, string Detail, short Rate) : ICommandRequest, IAuthorizationRequired;

public class ReviewCourseCommandHandler(
    ICourseRepository courseRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IAppLogger<ReviewCourseCommandHandler> logger
) : CommandHandlerBase<ReviewCourseCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(ReviewCourseCommand command, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetById(CourseId.Create(command.CourseId), cancellationToken);

        if (course is null) return DomainErrors.Courses.NotFound;

        var result = course.ReviewCourse(
            command.Rate,
            command.Detail,
            currentUserService.CurrentUserEmail);

        if (result.IsFailed) return result;

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}