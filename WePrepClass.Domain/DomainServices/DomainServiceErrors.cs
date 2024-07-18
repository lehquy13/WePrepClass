using Matt.ResultObject;

namespace WePrepClass.Domain.DomainServices;

public static class DomainServiceErrors
{
    public static readonly Error UserNotFound = new("UserNotFound", "User not found");
    public static readonly Error InvalidPassword = new("InvalidPassword", "Invalid password");
    public static readonly Error UserAlreadyExistDomainError = new("UserAlreadyExistDomainError", "User already exist");
    public static readonly Error EmailNotConfirmed = new("EmailNotConfirmed", "Email not confirmed");
    public static readonly Error RemoveUserFail = new("RemoveUserFail", "Remove user fail");
    public static readonly Error FailAddRoleError = new("FailAddRoleError", "Fail to add role");
}