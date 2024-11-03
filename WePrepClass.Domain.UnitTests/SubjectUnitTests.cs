using FluentAssertions;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;

namespace WePrepClass.Domain.UnitTests;

public class SubjectUnitTests
{
    private readonly Subject _subject = Subject.Create("Math", "Mathematics").Value;

    [Fact]
    public void CreateSubject_WhenNameAndDescriptionValid_ShouldCreatedSubject()
    {
        // Arrange
        const string name = "Valid name";
        const string description = "Valid description";

        // Act
        var subject = Subject.Create(name, description);

        // Assert
        subject.IsSuccess.Should().BeTrue();
        subject.Value.Name.Should().Be(name);
        subject.Value.Description.Should().Be(description);
    }

    [Fact]
    public void SetName_WhenNameIsTooShort_ShouldReturnError()
    {
        // Arrange
        const string name = "a";

        // Act
        var result = _subject.SetName(name);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }

    [Fact]
    public void SetName_WhenNameIsTooLong_ShouldReturnError()
    {
        // Arrange
        const string name = "This is a very long name that is longer than 30 characters";

        // Act
        var result = _subject.SetName(name);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }

    [Fact]
    public void SetSubjectDescription_WhenDescriptionIsTooShort_ShouldReturnError()
    {
        // Arrange
        const string description = "a";

        // Act
        var result = _subject.SetDescription(description);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }

    [Fact]
    public void SetDescription_WhenDescriptionIsTooLong_ShouldReturnError()
    {
        // Arrange
        const string description = "This is a very long description that is longer than 100 characters " +
                                   "and should not be allowed to be created as a subject description" +
                                   " because it is too long";

        // Act
        var result = _subject.SetDescription(description);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }

    [Fact]
    public void SetAsDeleted_WhenCalled_ShouldSetIsDeletedToTrue()
    {
        // Act
        _subject.SetAsDeleted();

        // Assert
        _subject.IsDeleted.Should().BeTrue();
    }
}