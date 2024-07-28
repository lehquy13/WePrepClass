using System.ComponentModel.DataAnnotations;
using Matt.ResultObject;
using Matt.SharedKernel;
using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Users;

public class User : FullAuditedAggregateRoot<UserId>
{
    private const int MaxBirthYear = 1990;
    private const int MinAge = 16;

    private const int MinDescriptionLength = 64;
    private const int MaxDescriptionLength = 256;

    private const int MaxPhoneNumberLength = 13;

    private const int MaxNameLength = 15;
    private const int MinNameLength = 2;

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Gender Gender { get; private set; } = Gender.Male;
    public int BirthYear { get; private set; } = 1990;
    public Address Address { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Avatar { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public Role Role { get; private set; } = Role.BaseUser;

    public const string DefaultAvatar = "https://res.cloudinary.com/dhehywasc/image/upload/v1697006256/male/male0.png";

    private User()
    {
    }

    public static Result<User> Create(
        UserId identityUserId,
        string firstName,
        string lastName,
        Gender gender,
        int birthYear,
        Address address,
        string description,
        string? avatar,
        string email,
        string phoneNumber,
        Role role)
    {
        var result = DomainValidation.Sequentially(
            () => IsBirthYearValid(birthYear) ? Result.Success() : DomainErrorConstants.User.InvalidBirthYear,
            () => IsNameValid(firstName) ? Result.Success() : DomainErrorConstants.User.FirstNameIsRequired,
            () => IsNameValid(lastName) ? Result.Success() : DomainErrorConstants.User.LastNameIsRequired,
            () => IsDescriptionValid(description) ? Result.Success() : DomainErrorConstants.User.DescriptionIsRequired,
            () => IsEmailValid(email) ? Result.Success() : DomainErrorConstants.User.EmailIsRequired,
            () => IsPhoneNumberValid(phoneNumber) ? Result.Success() : DomainErrorConstants.User.InvalidPhoneNumber
        );

        if (result.IsFailure) return result.Error;

        return new User
        {
            Id = identityUserId,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            BirthYear = birthYear,
            Address = address,
            Description = description,
            Avatar = string.IsNullOrWhiteSpace(avatar) ? DefaultAvatar : avatar,
            Email = email,
            PhoneNumber = phoneNumber,
            Role = role
        };
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public void SetAvatar(string avatarUrl) => Avatar = avatarUrl;

    private static bool IsPhoneNumberValid(string phoneNumber) =>
        phoneNumber.Length is <= MaxPhoneNumberLength &&
        (phoneNumber[0] == '+' && phoneNumber[..1].All(char.IsDigit)
         || phoneNumber.All(char.IsDigit));

    private static bool IsEmailValid(string email) => new EmailAddressAttribute().IsValid(email);

    private static bool IsBirthYearValid(int birthYear) =>
        birthYear >= MaxBirthYear && birthYear <= DateTime.Now.Year - MinAge;

    private static bool IsDescriptionValid(string description) =>
        description.Length is <= MaxDescriptionLength and >= MinDescriptionLength;

    private static bool IsNameValid(string name) => name.Length is <= MaxNameLength and >= MinNameLength;
}