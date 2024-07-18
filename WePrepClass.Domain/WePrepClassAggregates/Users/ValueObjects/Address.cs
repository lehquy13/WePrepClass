using Matt.SharedKernel.Domain.Primitives;

namespace WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

public class Address : ValueObject
{
    public string City { get; private init; } = string.Empty;

    public string Country { get; private init; } = string.Empty;

    private Address()
    {
    }

    public static Address Create(string city, string country)
    {
        return new Address
        {
            City = city,
            Country = country
        };
    }

    public override string ToString()
    {
        return $"City: {City}, Country: {Country}";
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return City;
        yield return Country;
    }

    public bool Match(string address)
    {
        return City.Contains(address, StringComparison.CurrentCultureIgnoreCase) ||
               Country.Contains(address, StringComparison.CurrentCultureIgnoreCase);
    }
}