using FluentAssertions;
using WePrepClass.Domain.WePrepClassAggregates.TutorRequests;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.UnitTests;

public class TutoringRequestUnitTests
{
    [Fact]
    public void CreateTutoringRequest_WhenTutorIsTheSameAsUser_ShouldReturnError()
    {
        // Arrange
        var tutorId = TutorId.Create();
        var userId = UserId.Create(tutorId.Value);
        const string message = "Hello, I need help with my course";

        // Act
        var result = TutoringRequest.Create(tutorId, userId, message);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(DomainErrors.TutoringRequests.CannotRequestTutoringThemselves);
    }

    [Fact]
    public void CreateTutoringRequest_WhenMessageLengthIsGreaterThan300_ShouldReturnError()
    {
        // Arrange
        var tutorId = TutorId.Create();
        var userId = UserId.Create();
        var invalidMessage = new string('a', 301);

        // Act
        var result = TutoringRequest.Create(tutorId, userId, invalidMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(DomainErrors.TutoringRequests.InvalidMessageLength);
    }
    
    [Fact]
    public void CreateTutoringRequest_WhenTutoringRequestIsValid_ShouldReturnTutoringRequest()
    {
        // Arrange
        var tutorId = TutorId.Create();
        var userId = UserId.Create();
        const string message = "Hello, I need help with my course";

        // Act
        var result = TutoringRequest.Create(tutorId, userId, message);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.TutorId.Should().Be(tutorId);
        result.Value.CourseId.Should().Be(userId);
        result.Value.Message.Should().Be(message);
    }
}