using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

public class UserId : ValueObject
{
    public Guid Value { get; private init; }

    private UserId()
    {
    }

    public static UserId Create(Guid guid = default) => new() { Value = guid == default ? Guid.NewGuid() : guid };

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}