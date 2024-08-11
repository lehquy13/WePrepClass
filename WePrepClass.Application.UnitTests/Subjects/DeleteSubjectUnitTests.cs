using FluentAssertions;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Application.UseCases.Administrator.Subjects.Commands;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

namespace WePrepClass.Application.UnitTests.Subjects;

public class DeleteSubjectUnitTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IAppLogger<DeleteSubjectCommandHandler>> _deleteSubjectLoggerMock = new();
    private readonly Mock<ISubjectRepository> _subjectRepositoryMock = new();

    private readonly DeleteSubjectCommandHandler _deleteSubjectCommandHandler;

    public DeleteSubjectUnitTests()
    {
        _deleteSubjectCommandHandler = new DeleteSubjectCommandHandler(
            _subjectRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _deleteSubjectLoggerMock.Object);
    }

    [Fact]
    public async Task DeleteSubject_WhenSubjectNotExists_ShouldReturnError()
    {
        // Arrange
        _subjectRepositoryMock
            .Setup(x =>
                x.GetAsync(It.IsAny<SubjectId>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject)null!);

        var deleteSubjectCommand = new DeleteSubjectCommand(1);

        // Act
        var result = await _deleteSubjectCommandHandler.Handle(deleteSubjectCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DeleteSubject_WhenSavingChangesFailed_ShouldReturnError()
    {
        // Arrange
        var subjectId = SubjectId.Create(1);
        var subject = Subject.Create("Math", "Mathematics").Value;
        var deleteSubjectCommand = new DeleteSubjectCommand(subjectId.Value);

        _subjectRepositoryMock
            .Setup(x =>
                x.GetAsync(subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _deleteSubjectCommandHandler.Handle(deleteSubjectCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DeleteSubject_WhenSubjectExists_ShouldSetAsDeleted()
    {
        // Arrange
        var subjectId = SubjectId.Create(1);
        var subject = Subject.Create("Math", "Mathematics").Value;
        var deleteSubjectCommand = new DeleteSubjectCommand(subjectId.Value);

        _subjectRepositoryMock
            .Setup(x => x.GetAsync(subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _deleteSubjectCommandHandler.Handle(deleteSubjectCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        subject.IsDeleted.Should().BeTrue();

        _subjectRepositoryMock.Verify(x => x.GetAsync(subjectId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}