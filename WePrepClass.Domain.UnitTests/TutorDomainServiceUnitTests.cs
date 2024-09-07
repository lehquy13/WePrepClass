using FluentAssertions;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.DomainServices;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.TutoringRequests;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.UnitTests;

public class TutorDomainServiceUnitTests
{
    private static readonly UserId ValidUserId = UserId.Create();

    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ITutorRepository> _tutorRepositoryMock = new();
    private readonly Mock<ITutoringRequestRepository> _tutoringRequestRepositoryMock = new();
    private readonly Mock<IAppLogger<TutorDomainService>> _loggerMock = new();

    private readonly TutorDomainService _tutorDomainService;

    private readonly Tutor _validTutor;

    public TutorDomainServiceUnitTests()
    {
        _tutorDomainService = new TutorDomainService(
            _currentUserServiceMock.Object,
            _tutorRepositoryMock.Object,
            _tutoringRequestRepositoryMock.Object,
            _loggerMock.Object
        );

        _validTutor = Tutor.Create(
            ValidUserId,
            AcademicLevel.Graduated,
            "University",
            [SubjectId.Create(1), SubjectId.Create(2)],
            TutorStatus.Active).Value;
    }


    [Fact]
    public async Task CreateTutoringRequest_WhenTutorIsNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var tutorId = TutorId.Create();

        _tutorRepositoryMock.Setup(x => x.GetById(tutorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Tutor);

        // Act
        var createTutoringRequestResult = await _tutorDomainService.CreateTutoringRequest(tutorId);

        // Assert
        createTutoringRequestResult.IsFailed.Should().BeTrue();
        createTutoringRequestResult.Error.Should().Be(DomainErrors.Tutors.NotFound);
    }

    [Fact]
    public async Task CreateTutoringRequest_WhenValidData_ShouldCreateRequestAndReturnSuccess()
    {
        // Arrange
        var tutor = _validTutor;

        _tutorRepositoryMock.Setup(x => x.GetById(tutor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tutor);

        // Act
        var createTutoringRequestResult = await _tutorDomainService.CreateTutoringRequest(tutor.Id);

        // Assert
        createTutoringRequestResult.IsSuccess.Should().BeTrue();
        _tutoringRequestRepositoryMock.Verify(x => x.Insert(It.IsAny<TutoringRequest>()), Times.Once);
    }
}