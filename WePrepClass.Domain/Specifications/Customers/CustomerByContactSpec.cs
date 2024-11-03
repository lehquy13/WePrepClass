using Matt.SharedKernel.Domain.Specifications;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Domain.Specifications.Customers;

public class CustomerByContactSpec : SpecificationBase<User>
{
    public CustomerByContactSpec(string contact)
    {
        Criteria = u => u.PhoneNumber == contact;
    }
}