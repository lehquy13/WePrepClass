using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Subscriptions.ValueObjects;

public class SubscriptionId : ValueObject
{
    private Guid Value { get; init; }

    private SubscriptionId(Guid value)
    {
        Value = value;
    }

    public static SubscriptionId Create(Guid value = default) => new(value == Guid.Empty ? Guid.NewGuid() : value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}