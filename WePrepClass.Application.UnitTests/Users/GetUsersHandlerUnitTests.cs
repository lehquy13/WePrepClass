using FluentAssertions;
using MapsterMapper;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using Moq.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Application.UseCases.Administrator.Users.Queries;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Users;

public class GetUsersHandlerUnitTests
{
    private readonly Mock<IReadDbContext> _readDbContextMock = new();
    private readonly Mock<IAppLogger<GetUsersQueryHandler>> _loggerMock = new();
    private readonly IMapper _mapperMock = MapsterProfile.Get;

    private const int PageIndex = 1;

    private readonly GetUsersQueryHandler _sut;

    public GetUsersHandlerUnitTests()
    {
        _sut = new GetUsersQueryHandler(
            _readDbContextMock.Object,
            _loggerMock.Object,
            _mapperMock
        );
    }

    [Fact]
    public async Task GetUsersHandler_WhenUsersExists_ShouldReturnUsers()
    {
        // Arrange
        List<User> users = [TestData.UserData.ValidUser];

        _readDbContextMock
            .Setup(x => x.Users)
            .ReturnsDbSet(users);

        var query = new GetUsersQuery(PageIndex);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().NotBeEmpty();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.Should().Contain(x => x.Id == TestData.UserData.ValidUser.Id.Value);
    }

    [Fact]
    public async Task GetUsersHandler_WhenUsersNotExists_ShouldReturnEmptyList()
    {
        // Arrange
        _readDbContextMock
            .Setup(x => x.Users)
            .ReturnsDbSet([]);

        var query = new GetUsersQuery(PageIndex);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().BeEmpty();
    }
}