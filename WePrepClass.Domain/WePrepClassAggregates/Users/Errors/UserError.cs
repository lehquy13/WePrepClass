using Matt.ResultObject;

namespace WePrepClass.Domain.WePrepClassAggregates.Users.Errors;

public static class UserError
{
    public static readonly Error NonExistUserError = new(
        "NonExistUserError",
        "This user doesn't exist!");
}