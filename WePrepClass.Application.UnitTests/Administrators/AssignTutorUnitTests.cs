using FluentAssertions;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Application.UseCases.Administrator.Courses.Commands;
using WePrepClass.Domain;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Administrators;

public class AssignTutorUnitTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<ITutorRepository> _tutorRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AssignTutorCommandHandler _handler;

    public AssignTutorUnitTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _tutorRepositoryMock = new Mock<ITutorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<IAppLogger<AssignTutorCommandHandler>>();

        _handler = new AssignTutorCommandHandler(
            _courseRepositoryMock.Object,
            _tutorRepositoryMock.Object,
            _unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenCourseIsNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var command = new AssignTutorCommand(Guid.NewGuid(), Guid.NewGuid());
        _courseRepositoryMock.Setup(x => x.GetById(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Course);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Courses.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTutorIsNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var course = TestData.CourseData.Course1;

        var command = new AssignTutorCommand(course.Id.Value, Guid.NewGuid());

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _tutorRepositoryMock.Setup(x => x.GetById(It.IsAny<TutorId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Tutor);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Tutors.NotFound);
    }
}