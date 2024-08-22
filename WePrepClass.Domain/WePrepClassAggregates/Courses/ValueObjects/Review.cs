using Matt.Auditing;
using Matt.ResultObject;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class Review : ValueObject, IHasModificationTime
{
    private const short MinRate = 1;
    private const short MaxRate = 5;
    private const int MaxDetailLength = 500;

    public short Rate { get; private init; }
    public string Detail { get; private init; } = null!;

    public DateTime? LastModificationTime { get; private init; }
    public string? LastModifierId { get; private set; }

    private Review()
    {
    }

    public static Result<Review> Create(short rate, string detail)
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
            LastModificationTime = DateTimeProvider.Now
        };
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Rate;
        yield return Detail;
    }
}