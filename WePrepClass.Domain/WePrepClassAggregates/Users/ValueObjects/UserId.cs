using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

public class UserId : ValueObject
{
    public Guid Value { get; private init; }

    protected UserId()
    {
    }

    public static UserId Create(Guid guid = default) => new() { Value = guid == default ? Guid.NewGuid() : guid };

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}