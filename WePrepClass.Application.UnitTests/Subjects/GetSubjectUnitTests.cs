using FluentAssertions;
using MapsterMapper;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Application.UseCases.Administrator.Subjects.Queries;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Application.UnitTests.Subjects;

public class GetSubjectUnitTests
{
    private readonly Mock<IAppLogger<GetSubjectQueryHandler>> _getSubjectLoggerMock = new();
    private readonly Mock<ISubjectRepository> _subjectRepositoryMock = new();

    private readonly IMapper _mapperMock = MapsterProfile.Get;
    private readonly GetSubjectQueryHandler _getSubjectQueryHandler;

    public GetSubjectUnitTests()
    {
        _getSubjectQueryHandler = new GetSubjectQueryHandler(
            _subjectRepositoryMock.Object,
            _getSubjectLoggerMock.Object,
            _mapperMock);
    }

    [Fact]
    public async Task GetSubject_WhenSubjectNotExists_ShouldReturnError()
    {
        // Arrange
        _subjectRepositoryMock
            .Setup(x =>
                x.GetAsync(It.IsAny<SubjectId>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject)null!);

        var getSubjectQuery = new GetSubjectQuery(1);

        // Act
        var result = await _getSubjectQueryHandler.Handle(getSubjectQuery, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetSubject_WhenSubjectExists_ShouldReturnSubject()
    {
        // Arrange
        var subjectId = SubjectId.Create(1);
        var subject = Subject.Create("Math", "Mathematics").Value;
        var getSubjectQuery = new GetSubjectQuery(subjectId.Value);

        _subjectRepositoryMock
            .Setup(x =>
                x.GetAsync(subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

        // Act
        var result = await _getSubjectQueryHandler.Handle(getSubjectQuery, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _subjectRepositoryMock.Verify(x => x.GetAsync(subjectId, It.IsAny<CancellationToken>()), Times.Once);
    }
}