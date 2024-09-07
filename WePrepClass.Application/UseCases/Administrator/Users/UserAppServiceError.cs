using Matt.ResultObject;

namespace WePrepClass.Application.UseCases.Administrator.Users;

public static class UserAppServiceError
{
    public static Error UserNotFound => new("UserNotFound", "User not found.");
}