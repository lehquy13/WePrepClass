using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Contracts.Courses;

public class CourseDto : BasicAuditedEntityDto<Guid>
{
    //Basic Information
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = CourseStatus.None.ToString();
    public string LearningMode { get; init; } = Domain.Commons.Enums.LearningMode.Offline.ToString();

    public string SectionFee { get; init; } = string.Empty;
    public string ChargeFee { get; init; } = string.Empty;

    //Tutor related information
    public string GenderRequirement { get; init; } = GenderOption.None.ToString();
    public string AcademicLevelRequirement { get; init; } = AcademicLevelOption.Optional.ToString();

    //Student related information
    public string LearnerGender { get; init; } = Gender.Female.ToString();
    public int NumberOfLearner { get; init; } = 1;

    // Time related information
    public int SessionDuration { get; init; } = 90;
    public int SessionPerWeek { get; init; } = 2;

    // Address related information
    public string Address { get; init; } = string.Empty;

    //Subject related information
    public int SubjectId { get; init; }
    public string SubjectName { get; init; } = string.Empty;
}