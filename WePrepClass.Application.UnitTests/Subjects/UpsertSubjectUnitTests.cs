using FluentAssertions;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Application.UseCases.Administrator.Subjects.Commands;
using WePrepClass.Contracts.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

namespace WePrepClass.Application.UnitTests.Subjects;

public class UpsertSubjectUnitTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IAppLogger<UpsertSubjectCommandHandler>> _deleteSubjectLoggerMock = new();
    private readonly Mock<ISubjectRepository> _subjectRepositoryMock = new();

    private readonly UpsertSubjectCommandValidator _upsertSubjectCommandValidator = new();
    private readonly UpsertSubjectCommandHandler _upsertSubjectCommandHandler;

    public UpsertSubjectUnitTests()
    {
        _upsertSubjectCommandHandler = new UpsertSubjectCommandHandler(
            _subjectRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _deleteSubjectLoggerMock.Object);
    }

    [Fact]
    public async Task UpsertSubjectCommandValidator_WhenSubjectDtoIsNull_ShouldReturnError()
    {
        // Arrange
        var upsertSubjectCommand = new UpsertSubjectCommand(null!);

        // Act
        var result = await _upsertSubjectCommandValidator.ValidateAsync(upsertSubjectCommand, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task UpsertSubjectCommandValidator_WhenSubjectDtoNameAndDescriptionAreEmpty_ShouldReturnError()
    {
        // Arrange
        var upsertSubjectCommand = new UpsertSubjectCommand(new SubjectDto());

        // Act
        var result = await _upsertSubjectCommandValidator.ValidateAsync(upsertSubjectCommand, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task UpsertSubject_WhenSubjectExists_ShouldSetSubject()
    {
        // Arrange
        const string nameAfterChange = "Name after changed";
        const string descriptionAfterChange = "Description after changed";

        var subjectId = SubjectId.Create(1);
        var subject = Subject.Create("Math", "Mathematics").Value;

        var upsertSubjectCommand = new UpsertSubjectCommand(new SubjectDto
        {
            Id = subjectId.Value,
            Name = nameAfterChange,
            Description = descriptionAfterChange
        });

        _subjectRepositoryMock
            .Setup(x => x.GetAsync(subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _upsertSubjectCommandHandler.Handle(upsertSubjectCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        subject.Name.Should().Be(nameAfterChange);
        subject.Description.Should().Be(descriptionAfterChange);

        _subjectRepositoryMock.Verify(x => x.GetAsync(subjectId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpsertSubject_WhenSubjectNotExists_ShouldCreateSubject()
    {
        // Arrange
        const string name = "Math";
        const string description = "Mathematics";

        var subjectId = SubjectId.Create(1);
        var upsertSubjectCommand = new UpsertSubjectCommand(new SubjectDto
        {
            Id = subjectId.Value,
            Name = name,
            Description = description
        });

        _subjectRepositoryMock
            .Setup(x => x.GetAsync(subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject)null!);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _upsertSubjectCommandHandler.Handle(upsertSubjectCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _subjectRepositoryMock.Verify(x => x.GetAsync(subjectId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpsertSubject_WhenSavingChangesFailed_ShouldReturnError()
    {
        // Arrange
        var subjectId = SubjectId.Create(1);
        var subject = Subject.Create("Math", "Mathematics").Value;
        var upsertSubjectCommand = new UpsertSubjectCommand(new SubjectDto
        {
            Id = subjectId.Value,
            Name = "Math",
            Description = "Mathematics"
        });

        _subjectRepositoryMock
            .Setup(x => x.GetAsync(subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _upsertSubjectCommandHandler.Handle(upsertSubjectCommand, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }
}