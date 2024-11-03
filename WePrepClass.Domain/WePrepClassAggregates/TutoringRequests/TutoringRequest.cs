using Matt.ResultObject;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.TutoringRequests.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.TutoringRequests;

public class TutoringRequest : AggregateRoot<TutorRequestId>
{
    private const int MaxMessageLength = 300;

    public TutorId TutorId { get; private set; } = null!;
    public UserId CourseId { get; private set; } = null!;
    public string Message { get; private set; } = null!;

    public TutoringRequestStatus TutorRequestStatus { get; private set; } = TutoringRequestStatus.InProgress;

    private TutoringRequest()
    {
    }

    public static Result<TutoringRequest> Create(TutorId tutorId, UserId userId, string message)
    {
        if (tutorId.Value == userId.Value) return DomainErrors.TutoringRequests.CannotRequestTutoringThemselves;

        if (message.Length > MaxMessageLength) return DomainErrors.TutoringRequests.InvalidMessageLength;

        var tutorRequest = new TutoringRequest
        {
            Id = TutorRequestId.Create(),
            TutorId = tutorId,
            CourseId = userId,
            Message = message
        };

        tutorRequest.DomainEvents.Add(new TutorRequestCreatedDomainEvent(tutorRequest));

        return tutorRequest;
    }

    public void CancelRequest()
    {
        TutorRequestStatus = TutoringRequestStatus.Cancelled;
    }

    public void CompleteRequest()
    {
        TutorRequestStatus = TutoringRequestStatus.Completed;
    }
}

public record TutorRequestCreatedDomainEvent(TutoringRequest TutoringRequest) : IDomainEvent;