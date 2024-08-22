using Matt.ResultObject;
using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;

public class VerificationChangeDetail : Entity<VerificationChangeDetailId>
{
    public string ImageUrl { get; private set; } = null!;

    public VerificationChangeId VerificationChangeId { get; private set; } = null!;

    private VerificationChangeDetail()
    {
    }

    public static Result<VerificationChangeDetail> Create(VerificationChangeId verificationChangeId, string imageUrl)
    {
        if (imageUrl.Length is 0) return DomainErrors.Tutors.InvalidImageUrl;

        return new VerificationChangeDetail
        {
            Id = VerificationChangeDetailId.Create(),
            VerificationChangeId = verificationChangeId,
            ImageUrl = imageUrl
        };
    }
}