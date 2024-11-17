using FluentAssertions;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Application.UseCases.Wpc.Profiles.Commands;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;

namespace WePrepClass.Application.UnitTests.Wpcs;

public class EnrollAsTutorHandlerUnitTests
{
    private readonly Mock<ITutorRepository> _tutorRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly EnrollAsTutorHandler _handler;

    public EnrollAsTutorHandlerUnitTests()
    {
        _tutorRepositoryMock = new Mock<ITutorRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<IAppLogger<EnrollAsTutorHandler>>();

        _handler = new EnrollAsTutorHandler(
            _tutorRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenValidData_ShouldReturnSuccess()
    {
        // Arrange
        var command = new EnrollAsTutor(
            AcademicLevel.Graduated,
            "University",
            [1, 2]
        );

        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _tutorRepositoryMock.Verify(x => x.Insert(It.IsAny<Tutor>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTutorCreationFails_ShouldReturnFailedResult()
    {
        // Arrange
        var command = new EnrollAsTutor(
            AcademicLevel.Graduated,
            "University",
            []
        );

        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }
}