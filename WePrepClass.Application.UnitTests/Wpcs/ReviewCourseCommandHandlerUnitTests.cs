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
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Wpcs;

public class ReviewCourseCommandHandlerUnitTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ReviewCourseCommandHandler _handler;

    public ReviewCourseCommandHandlerUnitTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<IAppLogger<ReviewCourseCommandHandler>>();

        _handler = new ReviewCourseCommandHandler(
            _courseRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenCourseIsNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var command = new ReviewCourseCommand(Guid.NewGuid(), "Detail message", 4);
        _courseRepositoryMock.Setup(x => x.GetById(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Course);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Courses.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCourseIsNotConfirmed_ShouldReturnFailedResult()
    {
        // Arrange
        var course = TestData.CourseData.Course1;

        var command = new ReviewCourseCommand(course.Id.Value, "Detail message", 4);
        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Courses.NotBeenConfirmed);
    }

    [Fact]
    public async Task Handle_WhenValidData_ShouldReturnSuccess()
    {
        // Arrange
        var course = TestData.CourseData.Course1;
        course.AssignTutor(TutorId.Create());

        DateTimeProvider.Set(DateTime.Now.AddDays(-40));

        course.ConfirmCourse();

        DateTimeProvider.Reset();

        var command = new ReviewCourseCommand(course.Id.Value, "Detail message", 4);
        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _currentUserServiceMock.Setup(x => x.CurrentUserEmail).Returns("user@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}