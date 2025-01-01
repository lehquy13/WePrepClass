using FluentAssertions;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Application.UseCases.Wpc.Courses.Commands;
using WePrepClass.Domain;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Wpcs;

public class CreateCourseRequestCommandHandlerUnitTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<ITutorRepository> _tutorRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateCourseRequestCommandHandler _handler;

    public CreateCourseRequestCommandHandlerUnitTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _tutorRepositoryMock = new Mock<ITutorRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateCourseRequestCommandHandler(
            _courseRepositoryMock.Object,
            _tutorRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenCourseNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var command = new CreateCourseRequestCommand(Guid.NewGuid());
        _courseRepositoryMock.Setup(x => x.GetById(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Course);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Courses.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCourseStatusIsNotAvailable_ShouldReturnFailedResult()
    {
        // Arrange
        var course = TestData.CourseData.Course1;
        course.SetCourseStatus(CourseStatus.Confirmed);

        var command = new CreateCourseRequestCommand(course.Id.Value);
        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Courses.Unavailable);
    }

    [Fact]
    public async Task Handle_WhenTutorNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var course = TestData.CourseData.Course1;
        course.SetCourseStatus(CourseStatus.Available);

        var command = new CreateCourseRequestCommand(course.Id.Value);
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

    [Fact]
    public async Task Handle_WhenValidData_ShouldReturnSuccess()
    {
        // Arrange
        var course = TestData.CourseData.Course1;
        course.SetCourseStatus(CourseStatus.Available);

        var tutor = Tutor.Create(
            UserId.Create(),
            AcademicLevel.Graduated,
            "University",
            new List<SubjectId> { SubjectId.Create(1), SubjectId.Create(2) },
            TutorStatus.Active).Value;

        var command = new CreateCourseRequestCommand(course.Id.Value);
        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _tutorRepositoryMock.Setup(x => x.GetById(It.IsAny<TutorId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tutor);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(tutor.UserId.Value);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}