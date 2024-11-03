using FluentAssertions;
using Matt.ResultObject;
using Moq;
using WePrepClass.Application.UseCases.Wpc.Tutors.Commands;
using WePrepClass.Domain;
using WePrepClass.Domain.DomainServices;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UnitTests.Tutors;

public class RequestTutoringCommandHandlerUnitTests
{
    private readonly Mock<ITutorDomainService> _tutorDomainServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RequestTutoringCommandHandler _handler;

    public RequestTutoringCommandHandlerUnitTests()
    {
        _tutorDomainServiceMock = new Mock<ITutorDomainService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<IAppLogger<RequestTutoringCommandHandler>>();

        _handler = new RequestTutoringCommandHandler(
            _tutorDomainServiceMock.Object,
            _unitOfWorkMock.Object,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenTutorIsNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var command = new RequestTutoringCommand(Guid.NewGuid(), "Detail message");

        _tutorDomainServiceMock.Setup(x =>
            x.CreateTutoringRequest(It.IsAny<TutorId>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(DomainErrors.Tutors.NotFound);

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
        var command = new RequestTutoringCommand(Guid.NewGuid(), "Detail message");

        _tutorDomainServiceMock.Setup(x =>
            x.CreateTutoringRequest(It.IsAny<TutorId>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}