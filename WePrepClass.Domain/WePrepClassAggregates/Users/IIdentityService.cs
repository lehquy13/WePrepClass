using Matt.ResultObject;
using Matt.SharedKernel.Application.Authorizations;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;
using Role = WePrepClass.Domain.Commons.Enums.Role;

namespace WePrepClass.Domain.WePrepClassAggregates.Users;

public interface IIdentityService : IDomainService
{
    /// <summary>
    /// Sign in with email and password
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<IdentityDto?> SignInAsync(string email, string password);

    /// <summary>
    /// Create new and empty user with default value: email, password, phoneNumber
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="gender"></param>
    /// <param name="birthYear"></param>
    /// <param name="address"></param>
    /// <param name="description"></param>
    /// <param name="avatar"></param>
    /// <param name="email"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    Task<Result<User>> CreateAsync(
        string userName,
        string firstName,
        string lastName,
        Gender gender,
        int birthYear,
        Address address,
        string description,
        string avatar,
        string email,
        string phoneNumber,
        Role role = Role.BaseUser);
    
    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="currentPassword"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    Task<Result> ChangePasswordAsync(UserId userId, string currentPassword, string newPassword);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="newPassword"></param>
    /// <param name="otpCode"></param>
    /// <returns></returns>
    Task<Result> ResetPasswordAsync(string email, string newPassword, string otpCode);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Result> RemoveAsync(UserId userId);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="commandEmail"></param>
    /// <returns></returns>
    Task<Result> ForgetPasswordAsync(string commandEmail);
}