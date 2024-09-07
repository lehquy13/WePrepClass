using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class Fee : ValueObject
{
    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = CurrencyCode.VND;

    private Fee()
    {
    }

    public static Fee Create(decimal amount, string? currency)
    {
        return new Fee
        {
            Amount = amount,
            Currency = currency ?? CurrencyCode.VND
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}