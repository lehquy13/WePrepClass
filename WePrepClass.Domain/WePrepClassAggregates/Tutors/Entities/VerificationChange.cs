using Matt.ResultObject;
using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;

public class VerificationChange : AuditedEntity<VerificationChangeId>
{
    private readonly List<VerificationChangeDetail> _verificationChangeDetails = [];

    public TutorId TutorId { get; private set; } = null!;
    public VerificationChangeStatus VerificationChangeStatus { get; private set; }

    public IReadOnlyCollection<VerificationChangeDetail> ChangeVerificationRequestDetails
        => _verificationChangeDetails.AsReadOnly();

    private VerificationChange()
    {
    }

    public static Result<VerificationChange> Create(TutorId tutorId, List<string> urls)
    {
        if (urls.Count is 0) return DomainErrors.Tutor.VerificationChangeCantBeEmpty;

        var verificationChanges = new VerificationChange
        {
            Id = VerificationChangeId.Create(),
            TutorId = tutorId,
            VerificationChangeStatus = VerificationChangeStatus.Pending
        };

        foreach (var verificationChangeDetail in urls.Select(url =>
                     VerificationChangeDetail.Create(verificationChanges.Id, url)))
        {
            if (verificationChangeDetail.IsFailed) return verificationChangeDetail.Error;

            verificationChanges._verificationChangeDetails.Add(verificationChangeDetail.Value);
        }

        return verificationChanges;
    }

    public void Approve()
    {
        VerificationChangeStatus = VerificationChangeStatus.Approved;
    }

    public void Reject()
    {
        VerificationChangeStatus = VerificationChangeStatus.Rejected;
    }
}