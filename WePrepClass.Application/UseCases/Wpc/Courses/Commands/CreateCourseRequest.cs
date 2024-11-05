using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain.DomainServices;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Courses.Commands;

public record CreateCourseRequestCommand(Guid CourseId) : ICommandRequest, IAuthorizationRequired;

public class CreateCourseRequestCommandHandler(
    ICourseDomainService courseDomainService,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IAppLogger<CreateCourseRequestCommandHandler> logger
) : CommandHandlerBase<CreateCourseRequestCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(CreateCourseRequestCommand command, CancellationToken cancellationToken)
    {
        var result = await courseDomainService.CreateTeachingRequest(
            CourseId.Create(command.CourseId),
            TutorId.Create(currentUserService.UserId), cancellationToken);

        if (result.IsFailed) return result;

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}