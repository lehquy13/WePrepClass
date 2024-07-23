using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

public class VerificationChangeDetailId : ValueObject
{
    public Guid Value { get; private init; }

    private VerificationChangeDetailId()
    {
    }

    public static VerificationChangeDetailId Create(Guid value = default)
    {
        return new VerificationChangeDetailId
        {
            Value = value == default ? Guid.NewGuid() : value
        };
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}