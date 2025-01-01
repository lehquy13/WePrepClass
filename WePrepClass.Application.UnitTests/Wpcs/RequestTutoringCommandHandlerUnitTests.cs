using FluentAssertions;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Application.UseCases.Wpc.Tutors.Commands;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.TutoringRequests;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Wpcs;

public class RequestTutoringCommandHandlerUnitTests
{
    private readonly Mock<ITutorRepository> _tutorRepositoryMock;
    private readonly Mock<ITutoringRequestRepository> _tutoringRequestRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RequestTutoringCommandHandler _handler;

    public RequestTutoringCommandHandlerUnitTests()
    {
        _tutorRepositoryMock = new Mock<ITutorRepository>();
        _tutoringRequestRepositoryMock = new Mock<ITutoringRequestRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RequestTutoringCommandHandler(
            _tutorRepositoryMock.Object,
            _tutoringRequestRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenTutorIsNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var command = new RequestTutoringCommand(Guid.NewGuid(), "Detail message");
        _tutorRepositoryMock.Setup(x => x.GetById(It.IsAny<TutorId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Tutor);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Tutors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTutorIsNotActive_ShouldReturnFailedResult()
    {
        // Arrange
        var inactiveTutor = TestData.TutorData.InActiveTutor;
        var command = new RequestTutoringCommand(Guid.NewGuid(), "Detail message");

        _tutorRepositoryMock.Setup(x => x.GetById(It.IsAny<TutorId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inactiveTutor);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Tutors.NotActive);
    }

    [Fact]
    public async Task Handle_WhenValidData_ShouldReturnSuccess()
    {
        // Arrange
        var tutor = TestData.TutorData.Tutor1;
        var command = new RequestTutoringCommand(Guid.NewGuid(), "Detail message");
        _tutorRepositoryMock.Setup(x => x.GetById(It.IsAny<TutorId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tutor);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _tutoringRequestRepositoryMock.Verify(x => x.Insert(It.IsAny<TutoringRequest>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}