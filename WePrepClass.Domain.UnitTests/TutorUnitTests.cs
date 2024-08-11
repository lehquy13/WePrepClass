using FluentAssertions;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;
using WePrepClass.UnitTestSetup;

namespace WePrepClass.Domain.UnitTests;

public class TutorUnitTests
{
    private readonly List<Subject> _subjects = TestData.SubjectData.Subjects;
    private readonly Tutor _validTutor;
    private readonly Tutor _validTutor2;

    public TutorUnitTests()
    {
        var userId = UserId.Create();
        var expectedMajor = _subjects[0];

        const AcademicLevel expectedAcademicLevel = AcademicLevel.UnderGraduate;
        const string university = "University of Lagos";
        var majors = new List<SubjectId> { expectedMajor.Id };

        var tutorCreationResult = Tutor.Create(userId, expectedAcademicLevel, university, majors);

        _validTutor = tutorCreationResult.Value;

        var userId2 = UserId.Create();
        var expectedMajor2 = _subjects[1];

        var majors2 = new List<SubjectId> { expectedMajor2.Id };

        var tutorCreationResult2 = Tutor.Create(userId2, expectedAcademicLevel, university, majors2);

        _validTutor2 = tutorCreationResult2.Value;

        var urls = new List<string> { "Document1", "Document2" };
        _validTutor2.ChangeVerification(urls);

        _validTutor2.SetProfileAsVerified();
    }

    [Fact]
    public void CreateTutorProfile_WhenWithValidData_ShouldCreateTutorProfile()
    {
        // Arrange
        var userId = UserId.Create();
        var expectedMajor = _subjects[0];

        const AcademicLevel expectedAcademicLevel = AcademicLevel.UnderGraduate;
        const string expectedUniversity = "University of Lagos";
        var majors = new List<SubjectId> { expectedMajor.Id };

        // Act
        var tutorCreationResult = Tutor.Create(userId, expectedAcademicLevel, expectedUniversity, majors);

        // Assert
        tutorCreationResult.IsSuccess.Should().BeTrue();
        tutorCreationResult.Value.Should().NotBeNull();

        var tutor = tutorCreationResult.Value;

        tutor.UserId.Should().NotBeNull();
        tutor.AcademicLevel.Should().Be(expectedAcademicLevel);
        tutor.University.Should().Be(expectedUniversity);
        tutor.Majors.Should().NotBeNull();
    }

    [Fact]
    public void CreateTutorProfile_WhenWithUniversityNameIsTooShort_ShouldNotCreateTutorProfile()
    {
        // Arrange
        var userId = UserId.Create();
        var expectedMajor = _subjects[0];

        const AcademicLevel expectedAcademicLevel = AcademicLevel.UnderGraduate;
        const string expectedUniversity = "Lo";
        var majors = new List<SubjectId> { expectedMajor.Id };

        // Act
        var tutorCreationResult = Tutor.Create(userId, expectedAcademicLevel, expectedUniversity, majors);

        // Assert
        tutorCreationResult.IsFailed.Should().BeTrue();
        tutorCreationResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void CreateTutorProfile_WhenWithUniversityNameIsTooLong_ShouldNotCreateTutorProfile()
    {
        // Arrange
        var userId = UserId.Create();
        var expectedMajor = _subjects[0];

        const AcademicLevel expectedAcademicLevel = AcademicLevel.UnderGraduate;
        const string expectedUniversity =
            "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been thenm.";
        var majors = new List<SubjectId> { expectedMajor.Id };

        // Act
        var tutorCreationResult = Tutor.Create(userId, expectedAcademicLevel, expectedUniversity, majors);

        // Assert
        tutorCreationResult.IsFailed.Should().BeTrue();
        tutorCreationResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void CreateTutorProfile_WhenWithNoMajors_ShouldNotCreateTutorProfile()
    {
        // Arrange
        var userId = UserId.Create();

        const AcademicLevel expectedAcademicLevel = AcademicLevel.UnderGraduate;
        const string expectedUniversity = "University of Lagos";

        // Act
        var tutorCreationResult = Tutor.Create(userId, expectedAcademicLevel, expectedUniversity, []);

        // Assert
        tutorCreationResult.IsFailed.Should().BeTrue();
        tutorCreationResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void CreateTutorProfile_WhenWithMoreThanFiveMajors_ShouldNotCreateTutorProfile()
    {
        // Arrange
        var userId = UserId.Create();

        const AcademicLevel expectedAcademicLevel = AcademicLevel.UnderGraduate;
        const string expectedUniversity = "University of Lagos";
        var majors = new List<SubjectId>
        {
            _subjects[0].Id,
            _subjects[1].Id,
            _subjects[2].Id,
            _subjects[3].Id,
            _subjects[4].Id,
            _subjects[5].Id
        };

        // Act
        var tutorCreationResult = Tutor.Create(userId, expectedAcademicLevel, expectedUniversity, majors);

        // Assert
        tutorCreationResult.IsFailed.Should().BeTrue();
        tutorCreationResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void UpdateTutorProfile_WhenWithValidData_ShouldUpdateTutorProfile()
    {
        // Arrange
        const AcademicLevel newAcademicLevel = AcademicLevel.Graduated;
        const string newUniversity = "University of Ibadan";
        var newMajors = new List<SubjectId> { _subjects[1].Id };

        // Act
        var tutorUpdateResult = _validTutor.Update(newUniversity, newAcademicLevel, newMajors);

        // Assert
        tutorUpdateResult.IsSuccess.Should().BeTrue();

        _validTutor.AcademicLevel.Should().Be(newAcademicLevel);
        _validTutor.University.Should().Be(newUniversity);
        _validTutor.Majors.Should().NotBeNull();
    }

    [Fact]
    public void UpdateTutorProfile_WhenWithUniversityNameIsTooShort_ShouldNotUpdateTutorProfile()
    {
        // Arrange
        const AcademicLevel newAcademicLevel = AcademicLevel.Graduated;
        const string newUniversity = "Lo";
        var newMajors = new List<SubjectId> { _subjects[1].Id };

        // Act
        var tutorUpdateResult = _validTutor.Update(newUniversity, newAcademicLevel, newMajors);

        // Assert
        tutorUpdateResult.IsFailed.Should().BeTrue();
        tutorUpdateResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void UpdateTutorProfile_WhenWithMajorIsEmpty_ShouldNotUpdateTutorProfile()
    {
        // Arrange
        const AcademicLevel newAcademicLevel = AcademicLevel.Graduated;
        const string newUniversity = "Lorem ";

        // Act
        var tutorUpdateResult = _validTutor.Update(newUniversity, newAcademicLevel, []);

        // Assert
        tutorUpdateResult.IsFailed.Should().BeTrue();
        tutorUpdateResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void SetRate_WhenWithValidRate_ShouldSetRate()
    {
        // Arrange
        const decimal expectedRate = 5;

        // Act
        var rateResult = _validTutor.SetRate(expectedRate);

        // Assert
        rateResult.IsSuccess.Should().BeTrue();
        _validTutor.Rate.Should().Be(expectedRate);
    }

    [Fact]
    public void SetRate_WhenWithInvalidRate_ShouldNotSetRate()
    {
        // Arrange
        const decimal expectedRate = -5;

        // Act
        var rateResult = _validTutor.SetRate(expectedRate);

        // Assert
        rateResult.IsFailed.Should().BeTrue();
        rateResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void SetRate_WhenWithRateIsTooHigh_ShouldNotSetRate()
    {
        // Arrange
        const decimal expectedRate = 6;

        // Act
        var rateResult = _validTutor.SetRate(expectedRate);

        // Assert
        rateResult.IsFailed.Should().BeTrue();
        rateResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void SetTutorStatus_WhenWithValidStatus_ShouldSetTutorStatus()
    {
        // Arrange
        const TutorStatus expectedStatus = TutorStatus.Active;

        // Act
        _validTutor.SetTutorStatus(expectedStatus);

        // Assert
        _validTutor.TutorStatus.Should().Be(expectedStatus);
    }

    [Fact]
    public void SetProfileAsVerified_WhenWithValidData_ShouldSetProfileAsVerified()
    {
        // Act
        _validTutor.SetProfileAsVerified();

        // Assert
        _validTutor.IsVerified.Should().BeTrue();
    }

    [Fact]
    public void ChangeVerification_WhenWithValidDataAndTutorHasExistedVerification_ShouldAddVerificationChange()
    {
        // Arrange
        var urls = new List<string> { "Document3", "Document4" };
        _validTutor2.ChangeVerification(urls);

        // Act
        var verificationResult = _validTutor2.ChangeVerification(urls);

        // Assert
        verificationResult.IsSuccess.Should().BeTrue();
        _validTutor2.VerificationChange.Should().NotBeNull();
        _validTutor2.VerificationChange.ChangeVerificationRequestDetails.Count.Should().Be(2);
    }

    [Fact]
    public void ChangeVerification_WhenWithValidDataAndTutorHasNoExistedVerification_ShouldSetVerification()
    {
        // Arrange
        var urls = new List<string> { "Document3", "Document4" };

        // Act
        var verificationResult = _validTutor.ChangeVerification(urls);

        // Assert
        verificationResult.IsSuccess.Should().BeTrue();

        _validTutor.VerificationChange.Should().BeNull();
        _validTutor.Verifications.Count.Should().Be(2);
        _validTutor.Verifications.All(v => urls.Contains(v.Image)).Should().BeTrue();
    }

    [Fact]
    public void ChangeVerification_WhenVerificationUrlEmpty_ShouldNotAddVerificationChange()
    {
        // Arrange
        var urls = new List<string>()
        {
            string.Empty,
            string.Empty
        };

        // Act
        var verificationResult = _validTutor.ChangeVerification(urls);

        // Assert
        verificationResult.IsFailed.Should().BeTrue();
        verificationResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void ChangeVerification_WhenEmptyVerificationUrl_ShouldNotAddVerificationChange()
    {
        // Arrange
        var urls = new List<string>();

        // Act
        var verificationResult = _validTutor.ChangeVerification(urls);

        // Assert
        verificationResult.IsFailed.Should().BeTrue();
        verificationResult.Error.Should().NotBeNull();
    }

    [Fact]
    public void VerifyVerificationChange_WhenApproved_ShouldUpdateVerification()
    {
        // Arrange
        var urls = new List<string> { "Document need to verify 1", "Document need to verify 2" };
        _validTutor2.ChangeVerification(urls);

        // Act
        var verificationResult = _validTutor2.VerifyVerificationChange(true);

        // Assert
        verificationResult.IsSuccess.Should().BeTrue();
        _validTutor2.VerificationChange.Should().NotBeNull();
        _validTutor2.Verifications.Count.Should().Be(2);
        _validTutor2.VerificationChange.VerificationChangeStatus.Should().Be(VerificationChangeStatus.Approved);
        _validTutor2.Verifications.All(v => urls.Contains(v.Image)).Should().BeTrue();
    }

    [Fact]
    public void VerifyVerificationChange_WhenRejected_ShouldNotUpdateVerification()
    {
        // Arrange
        var urls = new List<string> { "Document need to verify 1", "Document need to verify 2" };
        var oldVerifications = _validTutor2.Verifications;

        _validTutor2.ChangeVerification(urls);

        // Act
        var verificationResult = _validTutor2.VerifyVerificationChange(false);

        // Assert
        verificationResult.IsSuccess.Should().BeTrue();
        _validTutor2.VerificationChange.Should().NotBeNull();
        _validTutor2.VerificationChange.VerificationChangeStatus.Should().Be(VerificationChangeStatus.Rejected);
        _validTutor2.Verifications.Should().BeEquivalentTo(oldVerifications);
    }

    [Fact]
    public void VerifyVerificationChange_WhenNoVerificationChange_ShouldNotUpdateVerification()
    {
        // Act
        var verificationResult = _validTutor.VerifyVerificationChange(true);

        // Assert
        verificationResult.IsFailed.Should().BeTrue();
        verificationResult.Error.Should().NotBeNull();
    }
}