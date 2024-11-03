using FluentAssertions;
using MapsterMapper;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using Moq.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Application.UseCases.Wpc.Tutors.Queries;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Tutors;

public class GetTutorQueryHandlerUnitTests
{
    private readonly Mock<IReadDbContext> _dbContextMock;
    private readonly GetTutorQueryHandler _handler;

    public GetTutorQueryHandlerUnitTests()
    {
        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<IAppLogger<GetTutorQueryHandler>>();

        _dbContextMock = new Mock<IReadDbContext>();
        _handler = new GetTutorQueryHandler(
            _dbContextMock.Object,
            loggerMock.Object,
            mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenTutorExists_ShouldReturnTutorDto()
    {
        // Arrange
        var tutors = new List<Tutor> { TestData.TutorData.Tutor1 }.AsQueryable();
        var users = new List<User> { TestData.UserData.ValidUser }.AsQueryable();
        var subjects = TestData.SubjectData.Subjects.AsQueryable();
        var majors = new List<Major>() { TestData.MajorData.Major1 }.AsQueryable();

        var request = new GetTutorQuery(TestData.TutorData.Tutor1.Id.Value);

        _dbContextMock.Setup(db => db.Tutors).ReturnsDbSet(tutors);
        _dbContextMock.Setup(db => db.Users).ReturnsDbSet(users);
        _dbContextMock.Setup(db => db.Subjects).ReturnsDbSet(subjects);
        _dbContextMock.Setup(db => db.Majors).ReturnsDbSet(majors);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(TestData.TutorData.Tutor1.Id.Value);
        result.Value.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task Handle_WhenTutorDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var tutorId = Guid.NewGuid();
        var request = new GetTutorQuery(tutorId);

        _dbContextMock.Setup(db => db.Tutors).ReturnsDbSet(new List<Tutor>().AsQueryable());
        _dbContextMock.Setup(db => db.Users).ReturnsDbSet(new List<User>().AsQueryable());
        _dbContextMock.Setup(db => db.Subjects).ReturnsDbSet(new List<Subject>().AsQueryable());
        _dbContextMock.Setup(db => db.Majors).ReturnsDbSet(new List<Major>().AsQueryable());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(DomainErrors.Tutors.NotFound);
    }
}