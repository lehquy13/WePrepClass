using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Domain;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.TutoringRequests;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.DomainServices;

public interface ITutorDomainService : IDomainService
{
    Task<Result> CreateTutoringRequest(
        TutorId tutorId,
        string detailMessage = "",
        CancellationToken cancellationToken = default);
}

public class TutorDomainService(
    ICurrentUserService currentUserService,
    ITutorRepository tutorRepository,
    ITutoringRequestRepository tutoringRequestRepository,
    IAppLogger<TutorDomainService> logger
) : DomainServiceBase(logger), ITutorDomainService
{
    public async Task<Result> CreateTutoringRequest(
        TutorId tutorId,
        string detailMessage = "",
        CancellationToken cancellationToken = default)
    {
        var tutor = await tutorRepository.GetById(tutorId, cancellationToken);

        if (tutor is null) return DomainErrors.Tutors.NotFound;

        if (tutor.TutorStatus is not TutorStatus.Active) return DomainErrors.Tutors.NotActive;

        var tutoringRequest = TutoringRequest.Create(
            tutorId,
            UserId.Create(currentUserService.UserId),
            detailMessage);

        if (tutoringRequest.IsFailed) return tutoringRequest.Error;

        await tutoringRequestRepository.Insert(tutoringRequest.Value);

        return Result.Success();
    }
}