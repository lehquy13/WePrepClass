using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class TeachingAssignmentId : ValueObject
{
    public Guid Value { get; init; }

    private TeachingAssignmentId()
    {
    }

    public static TeachingAssignmentId Create(Guid guid = default) =>
        new() { Value = guid == default ? Guid.NewGuid() : guid };

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}