using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class CourseId : ValueObject
{
    public Guid Value { get; private set; }

    private CourseId()
    {
    }

    public static CourseId Create(Guid value = default)
    {
        return new CourseId { Value = value == default ? Guid.NewGuid() : value };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}