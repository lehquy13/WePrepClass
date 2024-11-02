using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain.DomainServices;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Tutors.Commands;

public record RequestTutoringCommand(
    Guid TutorId,
    string DetailMessage
) : ICommandRequest, IAuthorizationRequired;

public class RequestTutoringCommandHandler(
    ITutorDomainService tutorDomainService,
    IUnitOfWork unitOfWork,
    IAppLogger<RequestTutoringCommandHandler> logger
) : CommandHandlerBase<RequestTutoringCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(RequestTutoringCommand command,
        CancellationToken cancellationToken)
    {
        var result = await tutorDomainService.CreateTutoringRequest(
            TutorId.Create(command.TutorId), command.DetailMessage, cancellationToken);

        if (result.IsFailed) return result.Error;

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}