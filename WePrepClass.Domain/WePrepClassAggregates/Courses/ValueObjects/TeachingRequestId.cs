using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class TeachingRequestId : ValueObject
{
    public Guid Value { get; private set; }

    private TeachingRequestId(Guid value)
    {
        Value = value;
    }

    public static TeachingRequestId Create(Guid value = default) => new(value == Guid.Empty ? Guid.NewGuid() : value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}