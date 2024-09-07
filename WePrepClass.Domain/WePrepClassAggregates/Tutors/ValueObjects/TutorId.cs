using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

public class TutorId : ValueObject
{
    public Guid Value { get; private init; }

    private TutorId()
    {
    }

    public static TutorId Create(Guid value = default)
    {
        return new TutorId
        {
            Value = value == default ? Guid.NewGuid() : value
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}