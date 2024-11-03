using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

public class SubjectId : ValueObject
{
    public int Value { get; private init; }

    private SubjectId()
    {
    }

    public static SubjectId Create(int value = 0)
    {
        return new SubjectId
        {
            Value = value
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}