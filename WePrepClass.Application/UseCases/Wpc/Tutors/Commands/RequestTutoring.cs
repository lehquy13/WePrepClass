
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Results;
using WePrepClass.Domain;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.TutoringRequests;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Tutors.Commands;

public record RequestTutoringCommand(
    Guid TutorId,
    string DetailMessage
) : ICommandRequest, IAuthorizationRequired;

public class RequestTutoringCommandHandler(
    ITutorRepository tutorRepository,
    ITutoringRequestRepository tutoringRequestRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork
) : CommandHandlerBase<RequestTutoringCommand>(unitOfWork)
{
    public override async Task<Result> Handle(RequestTutoringCommand command,
        CancellationToken cancellationToken)
    {
        var tutorId = TutorId.Create(command.TutorId);

        var tutor = await tutorRepository.GetById(tutorId, cancellationToken);

        if (tutor is null) return DomainErrors.Tutors.NotFound;

        if (tutor.TutorStatus is not TutorStatus.Active) return DomainErrors.Tutors.NotActive;

        var tutoringRequest = TutoringRequest.Create(
            tutorId,
            UserId.Create(currentUserService.UserId),
            command.DetailMessage);

        if (tutoringRequest.IsFailed) return tutoringRequest.Error;

        await tutoringRequestRepository.Insert(tutoringRequest.Value);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}