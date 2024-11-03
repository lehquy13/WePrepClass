using FluentAssertions;
using Matt.SharedKernel.Domain.Interfaces;
using Moq;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.UnitTests.Courses;

public class CourseUnitTests
{
    private const string ValidTitle = "This is a valid course title that is more than 50 characters long";

    private static Course ValidCourse => Course.Create(
        ValidTitle,
        "This is a valid course description.",
        LearningMode.Offline,
        Fee.Create(100, CurrencyCode.VND),
        Fee.Create(10, CurrencyCode.VND),
        LearnerDetail.Create("Learner name", Gender.Female, "contact", 2, UserId.Create()),
        TutorSpecification.Create(GenderOption.Male, AcademicLevel.Graduated),
        Session.Create(60).Value,
        Address.Create("City", "District", "Street").Value,
        SubjectId.Create()
    ).Value;


    [Fact]
    public void CreateCourse_WhenWithValidData_ShouldSuccess()
    {
        // Arrange
        const string title = "This is a valid course title that is more than 50 characters long";
        const string description = "This is a valid course description.";
        const LearningMode learningMode = LearningMode.Offline;
        var sessionFee = Fee.Create(100, CurrencyCode.VND);
        var chargeFee = Fee.Create(10, CurrencyCode.VND);
        var learnerDetail = LearnerDetail.Create("Learner Name", Gender.Female, "contact");
        var tutorSpecification = TutorSpecification.Create(GenderOption.Male, AcademicLevel.Graduated);
        var sessionDuration = Session.Create(60);
        var address = Address.Create("City", "District", "Street");
        var subjectId = SubjectId.Create();

        // Act
        var courseCreationResult = Course.Create(
            title,
            description,
            learningMode,
            sessionFee,
            chargeFee,
            learnerDetail,
            tutorSpecification,
            sessionDuration.Value,
            address.Value,
            subjectId);

        // Assert
        courseCreationResult.IsSuccess.Should().BeTrue();

        var course = courseCreationResult.Value;

        course.Title.Should().Be(title);
        course.Description.Should().Be(description);
        course.LearningModeRequirement.Should().Be(learningMode);
        course.SessionFee.Should().Be(sessionFee);
        course.ChargeFee.Should().Be(chargeFee);
        course.LearnerDetail.Should().Be(learnerDetail);
        course.TutorSpecification.Should().Be(tutorSpecification);
        course.Session.Should().Be(sessionDuration.Value);
        course.Address.Should().Be(address.Value);
        course.SubjectId.Should().Be(subjectId);
    }

    [Fact]
    public void CreateCourse_WhenTitleIsShort_ShouldReturnFailedResult()
    {
        // Arrange
        const string title = "a";

        // Act
        var courseCreationResult = Course.Create(
            title,
            It.IsAny<string>(),
            It.IsAny<LearningMode>(),
            It.IsAny<Fee>(),
            It.IsAny<Fee>(),
            It.IsAny<LearnerDetail>(),
            It.IsAny<TutorSpecification>(),
            It.IsAny<Session>(),
            It.IsAny<Address>(),
            It.IsAny<SubjectId>());

        // Assert
        courseCreationResult.IsFailed.Should().BeTrue();
        courseCreationResult.Error.Should().Be(DomainErrors.Courses.TitleLengthOutOfRange);
    }

    [Fact]
    public void UpdateCourse_WhenTitleIsShort_ShouldReturnFailedResult()
    {
        // Arrange
        const string title = "a";

        // Act
        var updateCourseResult = ValidCourse.UpdateCourse(
            title,
            "This is a valid course description.",
            It.IsAny<LearningMode>(),
            It.IsAny<Fee>(),
            It.IsAny<Fee>(),
            It.IsAny<LearnerDetail>(),
            It.IsAny<TutorSpecification>(),
            It.IsAny<Session>(),
            It.IsAny<Address>(),
            It.IsAny<SubjectId>());

        // Assert
        updateCourseResult.IsFailed.Should().BeTrue();
        updateCourseResult.Error.Should().Be(DomainErrors.Courses.TitleLengthOutOfRange);
    }

    [Fact]
    public void UpdateCourse_WhenInputAllValid_ShouldReturnSuccessResult()
    {
        // Arrange

        // Act
        var updateCourseResult = ValidCourse.UpdateCourse(
            ValidTitle,
            "This is a valid course description.",
            It.IsAny<LearningMode>(),
            It.IsAny<Fee>(),
            It.IsAny<Fee>(),
            It.IsAny<LearnerDetail>(),
            It.IsAny<TutorSpecification>(),
            It.IsAny<Session>(),
            It.IsAny<Address>(),
            It.IsAny<SubjectId>());

        // Assert
        updateCourseResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ReviewCourse_WhenCourseConfirmedAfter30Days_ShouldReturnSuccessResult()
    {
        // Arrange
        const int expectedRate = 4;
        const string expectedComment = "This is a valid review comment";
        const string reviewer = "Reviewer";

        var course = ValidCourse;

        course.AssignTutor(tutorId: TutorId.Create());

        DateTimeProvider.Set(DateTime.Now.AddDays(-40));

        course.ConfirmCourse();

        DateTimeProvider.Reset();

        // Act
        var reviewCourseResult = course.ReviewCourse(expectedRate, expectedComment, reviewer);

        // Assert
        reviewCourseResult.IsSuccess.Should().BeTrue();

        course.Review.Should().NotBeNull();

        course.Review!.Rate.Should().Be(expectedRate);
        course.Review!.Detail.Should().Be(expectedComment);
    }

    [Fact]
    public void ReviewCourse_WhenCourseIsNotConfirmed_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;

        course.AssignTutor(tutorId: TutorId.Create());

        // Act
        var reviewCourseResult = course.ReviewCourse(5, "This is a valid review comment", "Reviewer");

        // Assert
        reviewCourseResult.IsFailed.Should().BeTrue();
        reviewCourseResult.Error.Should().Be(DomainErrors.Courses.NotBeenConfirmed);
    }

    [Fact]
    public void ReviewCourse_WhenConfirmedDateIsLessThan30Days_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;

        course.AssignTutor(tutorId: TutorId.Create());

        DateTimeProvider.Set(DateTime.Now.AddDays(-10));

        course.ConfirmCourse();

        DateTimeProvider.Reset();

        // Act
        var reviewCourseResult = course.ReviewCourse(5, "This is a valid review comment", "Reviewer");

        // Assert
        reviewCourseResult.IsFailed.Should().BeTrue();
        reviewCourseResult.Error.Should().Be(DomainErrors.Courses.ReviewNotAllowedYet);
    }

    [Fact]
    public void ReviewCourse_WhenRateIsGreaterThan5_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;

        // Act
        var reviewCourseResult = course.ReviewCourse(8, "This is a valid review comment", "Reviewer");

        // Assert
        reviewCourseResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void ReviewCourse_WhenRateIsSmallerThan0_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;

        // Act
        var reviewCourseResult = course.ReviewCourse(-1, "This is a valid review comment", "Reviewer");

        // Assert
        reviewCourseResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void ReviewCourse_WhenDetailIsEmpty_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;

        // Act
        var reviewCourseResult = course.ReviewCourse(4, "", "Reviewer");

        // Assert
        reviewCourseResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void ReviewCourse_WhenDetailIsTooLong_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;

        // Act
        var reviewCourseResult = course.ReviewCourse(4, new string('a', 501), "Reviewer");

        // Assert
        reviewCourseResult.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AssignTutor_WhenTutorAndLearnerAreTheSame_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;

        // Act
        var assignTutorResult = course.AssignTutor(TutorId.Create(course.LearnerDetail.LearnerId!.Value));

        // Assert
        assignTutorResult.IsFailed.Should().BeTrue();

        assignTutorResult.Error.Should().Be(DomainErrors.Courses.TutorAndLearnerShouldNotBeTheSame);
    }

    [Fact]
    public void AssignTutor_WhenThereIsNoRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var course = ValidCourse;

        // Act
        var assignTutorResult = course.AssignTutor(TutorId.Create());

        // Assert
        assignTutorResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void SetCourseStatus_WhenInputValidData_ShouldSuccess()
    {
        // Arrange
        var course = ValidCourse;

        // Act
        course.SetCourseStatus(CourseStatus.Refunded);

        // Assert
        course.Status.Should().Be(CourseStatus.Refunded);
    }

    [Fact]
    public void DissociateTutor_WhenStatusInvalid_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;
        const string note = "This is a valid detail message";

        course.SetCourseStatus(CourseStatus.Refunded);

        // Act
        var result = course.DissociateTutor(note);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Courses.StatusInvalidForUnassignment);
    }

    [Fact]
    public void DissociateTutor_WhenStatusValid_ShouldAddNoteAndSetTutorIdNull()
    {
        // Arrange
        var course = ValidCourse;
        const string note = "This is a valid detail message";

        course.AssignTutor(TutorId.Create());

        // Act
        var result = course.DissociateTutor(note);

        // Assert
        course.TutorId.Should().BeNull();
        course.Note.Should().Be(note);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ConfirmCourse_WhenCourseIsNotAssigned_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;

        // Act
        var result = course.ConfirmCourse();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Courses.HaveNotBeenAssigned);
    }

    [Fact]
    public void ConfirmCourse_WhenCourseIsAssigned_ShouldSuccess()
    {
        // Arrange
        var course = ValidCourse;

        course.AssignTutor(TutorId.Create());

        // Act
        var result = course.ConfirmCourse();

        // Assert
        result.IsSuccess.Should().BeTrue();
        course.Status.Should().Be(CourseStatus.Confirmed);
    }

    [Fact]
    public void RefundCourse_WhenCourseIsNotConfirmed_ShouldReturnFailedResult()
    {
        // Arrange
        var course = ValidCourse;

        // Act
        var result = course.RefundCourse("This is a valid command note");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Courses.Unavailable);
    }

    [Fact]
    public void RefundCourse_WhenCourseIsConfirmed_ShouldSuccess()
    {
        // Arrange
        var course = ValidCourse;

        course.AssignTutor(TutorId.Create());

        course.ConfirmCourse();

        // Act
        var result = course.RefundCourse("This is a valid command note");

        // Assert
        result.IsSuccess.Should().BeTrue();
        course.Status.Should().Be(CourseStatus.Refunded);
    }
}