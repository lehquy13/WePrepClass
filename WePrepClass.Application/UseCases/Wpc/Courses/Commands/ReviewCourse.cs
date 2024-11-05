using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain.DomainServices;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Courses.Commands;

public record ReviewCourseCommand(Guid CourseId, string Detail, short Rate) : ICommandRequest, IAuthorizationRequired;

public class ReviewCourseCommandHandler(
    ICourseDomainService courseDomainService,
    IUnitOfWork unitOfWork,
    IAppLogger<ReviewCourseCommandHandler> logger)
    : CommandHandlerBase<ReviewCourseCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(ReviewCourseCommand command, CancellationToken cancellationToken)
    {
        var result = await courseDomainService.ReviewCourse(
            CourseId.Create(command.CourseId),
            command.Rate,
            command.Detail);

        if (result.IsFailed) return result;

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}