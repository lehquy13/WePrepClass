using FluentAssertions;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.UnitTests;

public class UserUnitTests
{
    private const string FirstName = "John";
    private const string LastName = "Doe";
    private const Gender UserGender = Gender.Female;
    private const Role UserRole = Role.BaseUser;
    private const string Mail = "johnd@mail.com";
    private const string PhoneNumber = "0123456789";
    private const int BirthYear = 1991;
    private const string Description = "This is a valid description with length between 64 and 256 characters";
    private static readonly Address Address = Address.Create("Hanoi", "Ba Dinh", "123 Hoang Hoa Tham Street").Value;

    [Fact]
    public void CreateUser_WhenWithValidData_ShouldReturnUser()
    {
        // Arrange

        // Act
        var userResult = User.Create(
            UserId.Create(),
            FirstName,
            LastName,
            UserGender,
            BirthYear,
            Address,
            Description,
            null,
            Mail,
            PhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsSuccess.Should().BeTrue();
        userResult.Value.Should().NotBeNull();

        var user = userResult.Value;

        user.FirstName.Should().Be(FirstName);
        user.LastName.Should().Be(LastName);
        user.Gender.Should().Be(UserGender);
        user.BirthYear.Should().Be(BirthYear);
        user.Address.Should().Be(Address);
        user.Description.Should().Be(Description);
        user.Avatar.Should().Be(User.DefaultAvatar);
        user.Email.Should().Be(Mail);
        user.PhoneNumber.Should().Be(PhoneNumber);
        user.Role.Should().Be(UserRole);
    }

    [Fact]
    public void CreateUser_WhenWithInvalidData_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        var emptyLastName = string.Empty;

        // Act
        var userResult = User.Create(
            identityId,
            FirstName,
            emptyLastName,
            UserGender,
            BirthYear,
            Address,
            Description,
            null,
            Mail,
            PhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.LastNameIsRequired);
    }

    [Fact]
    public void CreateUser_WhenLastNameLengthGreaterThan20_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        var longLastName = new string('a', 21);

        // Act
        var userResult = User.Create(
            identityId,
            "John",
            longLastName,
            Gender.Female,
            1990,
            Address,
            Description,
            null,
            Mail,
            PhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.LastNameIsRequired);
    }

    [Fact]
    public void CreateUser_WhenFirstNameLengthLessThan2_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        const string shortFirstName = "J";

        // Act
        var userResult = User.Create(
            identityId,
            shortFirstName,
            LastName,
            Gender.Female,
            1990,
            Address,
            Description,
            null,
            Mail,
            PhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.FirstNameIsRequired);
    }

    [Fact]
    public void CreateUser_WhenFirstNameLengthGreaterThan20_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        var longFirstName = new string('a', 21);

        // Act
        var userResult = User.Create(
            identityId,
            longFirstName,
            LastName,
            Gender.Female,
            1990,
            Address,
            Description,
            null,
            Mail,
            PhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.FirstNameIsRequired);
    }

    [Fact]
    public void CreateUser_WhenWithInvalidBirthYear_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        const int invalidBirthYear = 1800; // Assuming birth year should be within a reasonable range

        // Act
        var userResult = User.Create(
            identityId,
            FirstName,
            LastName,
            Gender.Female,
            invalidBirthYear,
            Address,
            Description,
            null,
            Mail,
            PhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.InvalidBirthYear);
    }

    [Fact]
    public void CreateUser_WhenAgeIsLessThan16_ShouldReturnError()
    {
        // Arrange
        var birthYear = DateTime.Now.Year - 15;

        // Act
        var userResult = User.Create(
            UserId.Create(),
            FirstName,
            LastName,
            UserGender,
            birthYear,
            Address,
            Description,
            null,
            Mail,
            PhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.InvalidBirthYear);
    }

    [Fact]
    public void CreateUser_WhenUserIsTutorAndDescriptionLengthLessThan64_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        var shortDescription = new string('a', 63);
        const Role roleTutor = Role.Tutor;

        // Act
        var userResult = User.Create(
            identityId,
            FirstName,
            LastName,
            Gender.Female,
            1990,
            Address,
            shortDescription,
            null,
            Mail,
            PhoneNumber,
            roleTutor);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.DescriptionIsRequired);
    }

    [Fact]
    public void CreateUser_WhenUserIsTutorAndDescriptionLengthGreaterThan256_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        var longDescription = new string('a', 257);
        const Role roleTutor = Role.Tutor;

        // Act
        var userResult = User.Create(
            identityId,
            FirstName,
            LastName,
            Gender.Female,
            1990,
            Address,
            longDescription,
            null,
            Mail,
            PhoneNumber,
            roleTutor);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.DescriptionIsRequired);
    }

    [Fact]
    public void CreateUser_WhenWithInvalidEmail_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        const string invalidEmail = "invalid-email";

        // Act
        var userResult = User.Create(
            identityId,
            FirstName,
            LastName,
            Gender.Female,
            1990,
            Address,
            Description,
            null,
            invalidEmail,
            PhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.EmailIsRequired);
    }

    [Fact]
    public void CreateUser_WhenWithInvalidPhoneNumber_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        const string invalidPhoneNumber = "invalid-phone";

        // Act
        var userResult = User.Create(
            identityId,
            FirstName,
            LastName,
            Gender.Female,
            1990,
            Address,
            Description,
            null,
            Mail,
            invalidPhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.InvalidPhoneNumber);
    }

    [Fact]
    public void CreateUser_WhenPhoneNumberLengthGreaterThan13_ShouldReturnError()
    {
        // Arrange
        var identityId = UserId.Create();
        var longPhoneNumber = new string('1', 14);

        // Act
        var userResult = User.Create(
            identityId,
            FirstName,
            LastName,
            Gender.Female,
            1990,
            Address,
            Description,
            null,
            Mail,
            longPhoneNumber,
            UserRole);

        // Assert
        userResult.Should().NotBeNull();
        userResult.IsFailed.Should().BeTrue();
        userResult.Error.Should().Be(DomainErrors.User.InvalidPhoneNumber);
    }
}