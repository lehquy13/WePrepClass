using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class Fee : ValueObject
{
    public decimal Amount { get; private init; }

    public string Currency { get; private init; } = CurrencyCode.Vnd;
    
    public string Display => $"{Amount} {Currency}";

    private Fee()
    {
    }

    public static Fee Create(decimal amount, string? currency)
    {
        return new Fee
        {
            Amount = amount,
            Currency = currency ?? CurrencyCode.Vnd
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}