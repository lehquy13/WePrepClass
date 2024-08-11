using FluentAssertions;
using MapsterMapper;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Application.UseCases.Users.Queries;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Users;

public class GetUserByIdQueryHandlerUnitTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IAppLogger<GetUserByIdQueryHandler>> _loggerMock = new();
    private readonly IMapper _mapperMock = MapsterProfile.Get;

    private readonly GetUserByIdQueryHandler _sut;

    public GetUserByIdQueryHandlerUnitTests()
    {
        _sut = new GetUserByIdQueryHandler(
            _userRepositoryMock.Object,
            _loggerMock.Object,
            _mapperMock);
    }

    [Fact]
    public async Task GetUserByIdHandler_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(TestData.UserData.UserId, default))
            .ReturnsAsync(TestData.UserData.ValidUser);

        var query = new GetUserByIdQuery(TestData.UserData.UserId.Value);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        var user = result.Value;

        user.Id.Should().Be(TestData.UserData.UserId.Value);
        user.FullName.Should().Be($"{TestData.UserData.FirstName} {TestData.UserData.LastName}");
        user.Email.Should().Be(TestData.UserData.Mail);
        user.Gender.Should().Be(TestData.UserData.UserGender.ToString());
        user.BirthYear.Should().Be(TestData.UserData.BirthYear);
    }

    [Fact]
    public async Task GetUserByIdHandler_WhenUserNotExists_ShouldReturnError()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(TestData.UserData.UserId, default))
            .ReturnsAsync((User)null!);

        var query = new GetUserByIdQuery(TestData.UserData.UserId.Value);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }
}