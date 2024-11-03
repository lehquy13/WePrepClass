using Matt.SharedKernel.Domain.Specifications;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Domain.Specifications.Customers;

public class CustomerByEmailSpec : SpecificationBase<User>
{
    public CustomerByEmailSpec(string email)
    {
        Criteria = u => u.Email == email;
    }
}