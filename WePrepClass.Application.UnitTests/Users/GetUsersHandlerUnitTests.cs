using FluentAssertions;
using MapsterMapper;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Application.UseCases.Administrator.Users.Queries;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Users;

public class GetUsersHandlerUnitTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IAppLogger<GetUsersQueryHandler>> _loggerMock = new();
    private readonly IMapper _mapperMock = MapsterProfile.Get;

    private const int PageIndex = 1;
    private const int PageSize = 10;

    private readonly GetUsersQueryHandler _sut;

    public GetUsersHandlerUnitTests()
    {
        _sut = new GetUsersQueryHandler(
            _userRepositoryMock.Object,
            _loggerMock.Object,
            _mapperMock
        );
    }

    [Fact]
    public async Task GetUsersHandler_WhenUsersExists_ShouldReturnUsers()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetPaginatedListAsync(PageIndex, PageSize, default))
            .ReturnsAsync([TestData.UserData.ValidUser]);

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
        _userRepositoryMock
            .Setup(x => x.GetPaginatedListAsync(PageIndex, PageSize, default))
            .ReturnsAsync([]);

        var query = new GetUsersQuery(PageIndex);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().BeEmpty();
    }
}