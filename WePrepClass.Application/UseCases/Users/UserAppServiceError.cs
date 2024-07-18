using Matt.ResultObject;

namespace WePrepClass.Application.UseCases.Users;

public static class UserAppServiceError
{
    public static Error UserNotFound => new("UserNotFound", "User not found.");
}