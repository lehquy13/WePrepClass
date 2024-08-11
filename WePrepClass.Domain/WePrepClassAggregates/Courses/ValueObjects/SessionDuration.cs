using Matt.ResultObject;
using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class SessionDuration : ValueObject
{
    private const int MinDuration = 60;

    private int Value { get; init; }

    private DurationType DurationType { get; init; }

    public string DisplayValue => $"{Value} {DurationType}";

    private SessionDuration()
    {
    }

    public static Result<SessionDuration> Create(int value = 90, DurationType durationType = DurationType.Minute)
    {
        if (durationType is DurationType.Minute && value < MinDuration)
            return DomainErrors.Course.SessionDurationOutOfRange;

        return new SessionDuration
        {
            Value = value,
            DurationType = durationType
        };
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return DurationType;
    }
}