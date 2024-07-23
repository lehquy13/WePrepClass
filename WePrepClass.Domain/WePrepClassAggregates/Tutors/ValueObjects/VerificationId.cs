using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

public class VerificationId : ValueObject
{
    public Guid Value { get; private init; }

    private VerificationId()
    {
    }

    public static VerificationId Create(Guid value = default)
    {
        return new VerificationId
        {
            Value = value == default ? Guid.NewGuid() : value
        };
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}