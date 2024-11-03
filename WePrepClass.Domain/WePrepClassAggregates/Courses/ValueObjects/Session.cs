using Matt.ResultObject;
using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class Session : ValueObject
{
    private const int MinDuration = 60;

    public decimal Value { get; private init; }
    public DurationUnit DurationUnit { get; private init; }
    public SessionFrequency SessionFrequency { get; private init; } = SessionFrequency.Weekly;

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