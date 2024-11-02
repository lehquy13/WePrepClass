using Matt.ResultObject;
using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

public class Address : ValueObject
{
    public string City { get; private init; } = null!;

    public string District { get; private init; } = null!;

    public string DetailAddress { get; private init; } = null!;

    private Address()
    {
    }

    public static Result<Address> Create(string city, string country, string detail)
    {
        if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country) || string.IsNullOrWhiteSpace(detail))
            return Result.Fail("City, Country and Detail are required");

        return new Address
        {
            City = city,
            District = country,
            DetailAddress = detail
        };
    }

    public override string ToString() => $"{DetailAddress}, District: {District}, City: {City}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return City;
        yield return District;
    }

    public bool Match(string city, string district) =>
        City.Contains(city, StringComparison.CurrentCultureIgnoreCase) ||
        District.Contains(district, StringComparison.CurrentCultureIgnoreCase);
}