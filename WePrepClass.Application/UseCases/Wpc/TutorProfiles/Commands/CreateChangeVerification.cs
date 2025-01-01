using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Results;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.TutorProfiles.Commands;

public record CreateChangeVerificationCommand(List<string> ImageUrls) : ICommandRequest, IAuthorizationRequired;

public class CreateChangeVerificationCommandHandler(
    ITutorRepository dbContext,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork
) : CommandHandlerBase<CreateChangeVerificationCommand>(unitOfWork)
{
    public override async Task<Result> Handle(CreateChangeVerificationCommand command,
        CancellationToken cancellationToken)
    {
        var tutor = await dbContext.GetById(TutorId.Create(currentUserService.UserId), cancellationToken);

        if (tutor is null) return Result.Fail(DomainErrors.Tutors.NotFound);

        tutor.ChangeVerification(command.ImageUrls);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}