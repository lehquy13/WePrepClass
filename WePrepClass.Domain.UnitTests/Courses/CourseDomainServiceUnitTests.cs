using FluentAssertions;
using Moq;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.DomainServices;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.TeachingRequests;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.UnitTests.Courses;

public class CourseDomainServiceUnitTests
{
    private static readonly UserId ValidUserId = UserId.Create();
    private static readonly UserId LearnerId = UserId.Create();

    private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
    private readonly Mock<ITeachingRequestRepository> _teachingRequestRepositoryMock = new();
    private readonly Mock<ITutorRepository> _tutorRepositoryMock = new();

    private readonly CourseDomainService _courseDomainService;

    private static Course ValidCourse => Course.Create(
        "This is a valid course title that is more than 50 characters long",
        "This is a valid course description.",
        LearningMode.Offline,
        Fee.Create(100, CurrencyCode.VND),
        Fee.Create(10, CurrencyCode.VND),
        LearnerDetail.Create("Learner name", Gender.Female, "contact", 2, LearnerId),
        TutorSpecification.Create(GenderOption.Male, AcademicLevel.Graduated),
        Session.Create(60).Value,
        Address.Create("City", "District", "Street").Value,
        SubjectId.Create()
    ).Value;

    private readonly Tutor _validTutor;
    private readonly Tutor _invalidTutor;

    public CourseDomainServiceUnitTests()
    {
        _courseDomainService = new CourseDomainService(
            _courseRepositoryMock.Object,
            _teachingRequestRepositoryMock.Object,
            _tutorRepositoryMock.Object);

        _validTutor = Tutor.Create(
            ValidUserId,
            AcademicLevel.Graduated,
            "University",
            [SubjectId.Create(1), SubjectId.Create(2)],
            true).Value;

        _invalidTutor = Tutor.Create(
            LearnerId,
            AcademicLevel.Graduated,
            "University",
            [SubjectId.Create(1), SubjectId.Create(2)],
            true).Value;
    }

    [Fact]
    public async Task CreateTeachingRequest_WhenTutorHasRequested_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.Available);

        var tutor = _validTutor;

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _tutorRepositoryMock.Setup(x => x.GetById(tutor.Id, It.IsAny<CancellationToken>())).ReturnsAsync(tutor);

        _teachingRequestRepositoryMock
            .Setup(x => x.GetByCourseIdAndTutorId(course.Id, tutor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TeachingRequest.Create(tutor.Id, course.Id));

        // Act
        var createTeachingRequestResult = await _courseDomainService.CreateTeachingRequest(course.Id, tutor.Id);

        // Assert
        createTeachingRequestResult.IsFailed.Should().BeTrue();

        createTeachingRequestResult.Error.Should().Be(DomainErrors.Courses.TeachingRequestAlreadyExist);
    }

    [Fact]
    public async Task CreateTeachingRequest_WhenStatusOfCourseIsNotAvailable_ShouldReturnCourseUnavailable()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.Cancelled);

        var tutorId = TutorId.Create();

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var createTeachingRequestResult = await _courseDomainService.CreateTeachingRequest(course.Id, tutorId);

        // Assert
        createTeachingRequestResult.IsFailed.Should().BeTrue();

        createTeachingRequestResult.Error.Should().Be(DomainErrors.Courses.Unavailable);
    }

    [Fact]
    public async Task CreateTeachingRequest_WhenCourseIsNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var tutorId = TutorId.Create();
        var courseId = CourseId.Create();

        _courseRepositoryMock.Setup(x => x.GetById(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Course);

        // Act
        var createTeachingRequestResult = await _courseDomainService.CreateTeachingRequest(courseId, tutorId);

        // Assert
        createTeachingRequestResult.IsFailed.Should().BeTrue();

        createTeachingRequestResult.Error.Should().Be(DomainErrors.Courses.NotFound);
    }

    [Fact]
    public async Task CreateTeachingRequest_WhenTutorHaveNotRequest_ShouldCreateNewRequestAndReturnSuccess()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.Available);

        var tutor = _validTutor;

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _tutorRepositoryMock.Setup(x => x.GetById(tutor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tutor);

        _teachingRequestRepositoryMock
            .Setup(x => x.GetByCourseIdAndTutorId(course.Id, tutor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as TeachingRequest);

        // Act
        var createTeachingRequestResult = await _courseDomainService.CreateTeachingRequest(course.Id, tutor.Id);

        // Assert
        createTeachingRequestResult.IsSuccess.Should().BeTrue();

        _teachingRequestRepositoryMock.Verify(x => x.Insert(It.IsAny<TeachingRequest>()), Times.Once);
    }

    [Fact]
    public async Task AssignTutorToCourse_WhenCourseIsNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var tutorId = TutorId.Create();
        var courseId = CourseId.Create();

        _courseRepositoryMock.Setup(x => x.GetById(It.IsAny<CourseId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Course);

        // Act
        var assignTutorToCourseResult = await _courseDomainService.AssignTutorToCourse(courseId, tutorId);

        // Assert
        assignTutorToCourseResult.IsFailed.Should().BeTrue();

        assignTutorToCourseResult.Error.Should().Be(DomainErrors.Courses.NotFound);
    }

    [Fact]
    public async Task AssignTutorToCourse_WhenTutorIsNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.Available);

        var tutorId = TutorId.Create();

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _tutorRepositoryMock.Setup(x => x.GetById(tutorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Tutor);

        // Act
        var assignTutorToCourseResult = await _courseDomainService.AssignTutorToCourse(course.Id, tutorId);

        // Assert
        assignTutorToCourseResult.IsFailed.Should().BeTrue();

        assignTutorToCourseResult.Error.Should().Be(DomainErrors.Tutors.NotFound);
    }

    [Fact]
    public async Task AssignTutorToCourse_WhenTutorAndLearnerAreTheSame_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.Available);

        var tutorId = _invalidTutor.Id;

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        _tutorRepositoryMock.Setup(x => x.GetById(tutorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_invalidTutor);

        _teachingRequestRepositoryMock
            .Setup(x => x.GetTeachingRequestsByCourseId(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var assignTutorToCourseResult = await _courseDomainService.AssignTutorToCourse(course.Id, tutorId);

        // Assert
        assignTutorToCourseResult.IsFailed.Should().BeTrue();

        assignTutorToCourseResult.Error.Should().Be(DomainErrors.Courses.TutorAndLearnerShouldNotBeTheSame);
    }

    [Fact]
    public async Task AssignTutorToCourse_WhenCourseIsNotAssigned_ShouldAssignTutorAndReturnSuccess()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.Available);

        var tutor = _validTutor;

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _tutorRepositoryMock.Setup(x => x.GetById(tutor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tutor);

        _teachingRequestRepositoryMock
            .Setup(x => x.GetTeachingRequestsByCourseId(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var assignTutorToCourseResult = await _courseDomainService.AssignTutorToCourse(course.Id, tutor.Id);

        // Assert
        assignTutorToCourseResult.IsSuccess.Should().BeTrue();

        course.TutorId.Should().Be(tutor.Id);
        course.Status.Should().Be(CourseStatus.InProgress);
    }

    [Fact]
    public async Task AssignTutorToCourse_WhenTeachingRequestExist_ShouldApproveRequestAndCancelOthers()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.Available);

        var tutor = _validTutor;
        var teachingRequest = TeachingRequest.Create(tutor.Id, course.Id);
        var teachingRequestToBeCancelled = TeachingRequest.Create(TutorId.Create(), course.Id);

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _tutorRepositoryMock.Setup(x => x.GetById(tutor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tutor);

        _teachingRequestRepositoryMock
            .Setup(x => x.GetTeachingRequestsByCourseId(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                teachingRequest,
                teachingRequestToBeCancelled
            ]);

        // Act
        var assignTutorToCourseResult = await _courseDomainService.AssignTutorToCourse(course.Id, tutor.Id);

        // Assert
        assignTutorToCourseResult.IsSuccess.Should().BeTrue();

        course.TutorId.Should().Be(tutor.Id);
        course.Status.Should().Be(CourseStatus.InProgress);

        teachingRequest.TeachingRequestStatus.Should().Be(RequestStatus.Approved);
        teachingRequestToBeCancelled.TeachingRequestStatus.Should().Be(RequestStatus.Denied);
    }

    [Fact]
    public async Task DissociateTutorFromCourse_WhenCourseNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        var courseId = CourseId.Create();

        const string note = "Note";

        _courseRepositoryMock.Setup(x => x.GetById(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Course);

        // Act
        var dissociateTutorFromCourseResult =
            await _courseDomainService.DissociateTutor(courseId, note);

        // Assert
        dissociateTutorFromCourseResult.IsFailed.Should().BeTrue();

        dissociateTutorFromCourseResult.Error.Should().Be(DomainErrors.Courses.NotFound);
    }

    [Fact]
    public async Task DissociateTutorFromCourse_WhenCourseStatusIsInvalid_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.Cancelled);

        const string note = "Note";

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var dissociateTutorFromCourseResult =
            await _courseDomainService.DissociateTutor(course.Id, note);

        // Assert
        dissociateTutorFromCourseResult.IsFailed.Should().BeTrue();

        dissociateTutorFromCourseResult.Error.Should().Be(DomainErrors.Courses.StatusInvalidForUnassignment);
    }

    [Fact]
    public async Task DissociateTutorFromCourse_WhenTutorIsNotAssigned_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.InProgress);

        const string note = "Note";

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var dissociateTutorFromCourseResult =
            await _courseDomainService.DissociateTutor(course.Id, note);

        // Assert
        dissociateTutorFromCourseResult.IsFailed.Should().BeTrue();

        dissociateTutorFromCourseResult.Error.Should().Be(DomainErrors.Courses.HaveNotBeenAssigned);
    }

    [Fact]
    public async Task DissociateTutorFromCourse_WhenTutorIsAssigned_ShouldDissociateTutorAndReturnSuccess()
    {
        // Arrange
        var course = ValidCourse;
        course.SetCourseStatus(CourseStatus.Available);
        course.AssignTutor(_validTutor.Id);

        var teachingRequest = TeachingRequest.Create(_validTutor.Id, course.Id);

        const string note = "Note";

        _courseRepositoryMock.Setup(x => x.GetById(course.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        _teachingRequestRepositoryMock
            .Setup(x => x.GetByCourseIdAndTutorId(course.Id, _validTutor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teachingRequest);

        // Act
        var dissociateTutorFromCourseResult =
            await _courseDomainService.DissociateTutor(course.Id, note);

        // Assert
        dissociateTutorFromCourseResult.IsSuccess.Should().BeTrue();

        course.TutorId.Should().BeNull();
        course.Note.Should().Be(note);

        teachingRequest.TeachingRequestStatus.Should().Be(RequestStatus.Denied);
    }
}