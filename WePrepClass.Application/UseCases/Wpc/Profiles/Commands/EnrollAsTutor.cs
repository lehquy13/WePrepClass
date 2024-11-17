using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Profiles.Commands;

public record EnrollAsTutor(
    AcademicLevel AcademicLevel,
    string University,
    List<int> MajorIds
    //List<string> imageFileUrls, TODO: update domain event to push notification to update image
) : ICommandRequest, IAuthorizationRequired;

public class EnrollAsTutorHandler(
    ITutorRepository tutorRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IAppLogger<EnrollAsTutorHandler> logger
) : CommandHandlerBase<EnrollAsTutor>(unitOfWork, logger)
{
    public override async Task<Result> Handle(EnrollAsTutor command, CancellationToken cancellationToken)
    {
        var result = Tutor.Create(
            UserId.Create(currentUserService.UserId),
            command.AcademicLevel,
            command.University,
            command.MajorIds.Select(SubjectId.Create).ToList());

        if (result.IsFailed) return result.Error;

        tutorRepository.Insert(result.Value);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}