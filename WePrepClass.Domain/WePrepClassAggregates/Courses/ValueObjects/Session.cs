using Matt.ResultObject;
using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class Session : ValueObject
{
    private const int MinDuration = 60;

    private decimal Value { get; init; }
    private DurationUnit DurationUnit { get; init; }
    private SessionFrequency SessionFrequency { get; init; } = SessionFrequency.Weekly;

    public string DisplayValue => SessionFrequency is SessionFrequency.Custom
        ? $"{Value} {DurationUnit}"
        : $"{Value} {DurationUnit} per {SessionFrequency}";

    private Session()
    {
    }

    public static Result<Session> Create(decimal value = 90m, DurationUnit durationUnit = DurationUnit.Minute)
    {
        if (durationUnit is DurationUnit.Minute && value < MinDuration)
            return DomainErrors.Courses.SessionDurationOutOfRange;

        return new Session
        {
            Value = value,
            DurationUnit = durationUnit
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return DurationUnit;
    }
}