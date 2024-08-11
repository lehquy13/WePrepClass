using Matt.ResultObject;
using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class SessionPerWeek : ValueObject
{
    private int Value { get; init; }

    private SessionPerWeek()
    {
    }

    public static Result<SessionPerWeek> Create(int value = 0)
    {
        if (value is <= 0 or > 7) return DomainErrors.Course.SessionPerWeekOutOfRange;

        return new SessionPerWeek
        {
            Value = value
        };
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}