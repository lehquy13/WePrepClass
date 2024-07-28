using FluentAssertions;
using WePrepClass.Application.UseCases.Users.Commands;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Application.UnitTests.Users;

public class CreateUserCommandValidatorUnitTests
{
    private const string UserName = "JohnDoe";
    private const string FirstName = "John";
    private const string LastName = "Doe";
    private const string Password = "1q2w3E**";
    private const Gender UserGender = Gender.Female;
    private const string Mail = "johnd@mail.com";
    private const string PhoneNumber = "0123456789";
    private const int BirthYear = 1991;
    private const string City = "Hanoi";
    private const string District = "Ba Dinh";
    private const string Street = "123 Hoang Hoa Tham Street";

    private readonly CreateUserCommandValidator _validator = new();


    [Fact]
    public void ValidateCreateUserCommand_WhenValid_ShouldPass()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            Password,
            FirstName,
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenUserNameIsInvalid_ShouldFail()
    {
        // Arrange
        const string invalidUserName = "John Doe";

        var command = new CreateUserCommand(
            invalidUserName,
            Mail,
            PhoneNumber,
            Password,
            FirstName,
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.Username));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenUsernameLengthLessThan6_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            "user",
            Mail,
            PhoneNumber,
            Password,
            FirstName,
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.Username));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenUsernameLengthGreaterThan20_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            new string('a', 21),
            Mail,
            PhoneNumber,
            Password,
            FirstName,
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.Username));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenUsernameDoesNotMatchRegex_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            "invalid username",
            Mail,
            PhoneNumber,
            Password,
            FirstName,
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.Username));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenEmailIsInvalid_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            "invalid-email",
            PhoneNumber,
            Password,
            FirstName,
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.Email));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenPhoneNumberDoesNotMatchRegex_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            "invalid-phone",
            Password,
            FirstName,
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.PhoneNumber));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenPasswordLengthLessThan8_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            "Pass1!",
            FirstName,
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.Password));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenPasswordDoesNotMatchRegex_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            "password",
            FirstName,
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.Password));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenFirstNameIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            Password,
            "",
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.FirstName));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenFirstNameLengthLessThanMin_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            Password,
            "J",
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.FirstName));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenFirstNameLengthGreaterThanMax_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            Password,
            new string('a', 21),
            LastName,
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.FirstName));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenLastNameIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            Password,
            FirstName,
            "",
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.LastName));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenLastNameLengthLessThanMin_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            Password,
            FirstName,
            "D",
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.LastName));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenLastNameLengthGreaterThan20_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            Password,
            FirstName,
            new string('a', 21),
            BirthYear,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.LastName));
    }

    [Fact]
    public void ValidateCreateUserCommand_WhenBirthYearNotInRange_ShouldFail()
    {
        // Arrange
        var command = new CreateUserCommand(
            UserName,
            Mail,
            PhoneNumber,
            Password,
            FirstName,
            LastName,
            1800,
            City,
            District,
            Street,
            UserGender);

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserCommand.BirthYear));
    }
}