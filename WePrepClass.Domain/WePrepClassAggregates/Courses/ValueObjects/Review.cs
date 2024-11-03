using Matt.Auditing;
using Matt.ResultObject;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class Review : ValueObject, ICreationAuditedObject, IModificationAuditedObject
{
    private const short MinRate = 1;
    private const short MaxRate = 5;
    public const int MaxDetailLength = 500;

    public short Rate { get; private init; }
    public string Detail { get; private init; } = null!;

    public DateTime? LastModificationTime { get; private init; }
    public string? LastModifierId { get; private set; }

    public DateTime CreationTime { get; private init; }

    public string? CreatorId { get; private init; }

    private Review()
    {
    }

    public static Result<Review> Create(short rate, string detail, string reviewer)
    {
        if (rate is < MinRate or > MaxRate)
        {
            return Result.Fail(DomainErrors.Courses.InvalidReviewRate);
        }

        if (detail.Length > MaxDetailLength)
        {
            return Result.Fail(DomainErrors.Courses.InvalidDetailLength);
        }

        return new Review
        {
            Rate = rate,
            Detail = detail,
            CreatorId = reviewer,
            LastModificationTime = DateTimeProvider.Now,
            CreationTime = DateTimeProvider.Now
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Rate;
        yield return Detail;
    }
}