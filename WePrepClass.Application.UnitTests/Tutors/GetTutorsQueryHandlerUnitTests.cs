using FluentAssertions;
using MapsterMapper;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using Moq.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Application.UseCases.Wpc.Tutors.Queries;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Tutors;

public class GetTutorsQueryHandlerUnitTests
{
    private readonly Mock<IReadDbContext> _dbContextMock;
    private readonly GetTutorsQueryHandler _handler;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();

    public GetTutorsQueryHandlerUnitTests()
    {
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<GetTutorsQueryHandler>>();

        _dbContextMock = new Mock<IReadDbContext>();
        _handler = new GetTutorsQueryHandler(
            _dbContextMock.Object,
            _currentUserServiceMock.Object,
            loggerMock.Object,
            mapperMock.Object
        );

        var tutors = new List<Tutor> { TestData.TutorData.Tutor1 }.AsQueryable();
        var users = new List<User> { TestData.UserData.ValidUser }.AsQueryable();
        var subjects = TestData.SubjectData.Subjects.AsQueryable();
        var majors = new List<Major> { TestData.MajorData.Major1 }.AsQueryable();
        var courses = new List<Course> { TestData.CourseData.Course1 }.AsQueryable();

        _dbContextMock.Setup(db => db.Tutors).ReturnsDbSet(tutors);
        _dbContextMock.Setup(db => db.Users).ReturnsDbSet(users);
        _dbContextMock.Setup(db => db.Subjects).ReturnsDbSet(subjects);
        _dbContextMock.Setup(db => db.Majors).ReturnsDbSet(majors);
        _dbContextMock.Setup(db => db.Courses).ReturnsDbSet(courses);
    }

    [Fact]
    public async Task Handle_WhenTutorsExistAndUserDoesNotLogin_ShouldReturnPaginatedListOfTutorListDto()
    {
        // Arrange
        var request = new GetTutorsQuery(new GetTutorsRequest
        {
            PageIndex = 1,
            PageSize = 10
        });

        _currentUserServiceMock.Setup(x => x.IsAuthenticated).Returns(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.First().Id.Should().Be(TestData.TutorData.Tutor1.Id.Value);
    }

    [Fact]
    public async Task Handle_WhenNoTutorsExist_ShouldReturnEmptyPaginatedList()
    {
        // Arrange
        var request = new GetTutorsQuery(new GetTutorsRequest
        {
            PageIndex = 1,
            PageSize = 10
        });

        _dbContextMock.Setup(db => db.Tutors).ReturnsDbSet(new List<Tutor>().AsQueryable());
        _dbContextMock.Setup(db => db.Users).ReturnsDbSet(new List<User>().AsQueryable());
        _dbContextMock.Setup(db => db.Subjects).ReturnsDbSet(new List<Subject>().AsQueryable());
        _dbContextMock.Setup(db => db.Majors).ReturnsDbSet(new List<Major>().AsQueryable());
        _dbContextMock.Setup(db => db.Courses).ReturnsDbSet(new List<Course>().AsQueryable());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
    }
}