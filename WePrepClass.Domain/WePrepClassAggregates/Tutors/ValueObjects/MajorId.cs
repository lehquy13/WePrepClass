using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

public class MajorId : ValueObject
{
    public Guid Value { get; private init; }

    private MajorId()
    {
    }

    public static MajorId Create(Guid value = default)
    {
        return new MajorId
        {
            Value = value == default ? Guid.NewGuid() : value
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}