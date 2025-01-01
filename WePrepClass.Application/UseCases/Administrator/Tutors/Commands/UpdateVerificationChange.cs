using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Results;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Administrator.Tutors.Commands;

public record UpdateVerificationChangeCommand(Guid TutorId, bool IsApproved) : ICommandRequest;

public class UpdateChangeVerificationCommandHandler(
    ITutorRepository tutorRepository,
    IUnitOfWork unitOfWork
) : CommandHandlerBase<UpdateVerificationChangeCommand>(unitOfWork)
{
    public override async Task<Result> Handle(UpdateVerificationChangeCommand command,
        CancellationToken cancellationToken)
    {
        var tutor = await tutorRepository.GetById(TutorId.Create(command.TutorId), cancellationToken);

        if (tutor is null) return Result.Fail(DomainErrors.Tutors.NotFound);

        var result = tutor.VerifyVerificationChange(command.IsApproved);

        if (result.IsFailed) return Result.Fail(result.Error);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}