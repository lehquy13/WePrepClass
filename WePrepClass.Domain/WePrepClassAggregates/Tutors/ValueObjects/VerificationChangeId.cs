using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

public class VerificationChangeId : ValueObject
{
    public Guid Value { get; private init; }

    private VerificationChangeId()
    {
    }

    public static VerificationChangeId Create(Guid value = default)
    {
        return new VerificationChangeId
        {
            Value = value == default ? Guid.NewGuid() : value
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}