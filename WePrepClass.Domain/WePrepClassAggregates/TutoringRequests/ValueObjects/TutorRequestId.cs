using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.TutoringRequests.ValueObjects;

public class TutorRequestId : ValueObject
{
    private Guid Value { get; init; }

    private TutorRequestId()
    {
    }

    public static TutorRequestId Create(Guid guid = default) =>
        new() { Value = guid == default ? Guid.NewGuid() : guid };

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}