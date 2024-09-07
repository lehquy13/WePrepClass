using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Payments.ValueObjects;

public class PaymentId : ValueObject
{
    private Guid Value { get; init; }

    private PaymentId(Guid value)
    {
        Value = value;
    }

    public static PaymentId Create(Guid value = default) => new(value == Guid.Empty ? Guid.NewGuid() : value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}