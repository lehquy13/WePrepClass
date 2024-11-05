using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Contracts.Courses;

public sealed class CourseListDto : BasicAuditedEntityDto<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = CourseStatus.None.ToString();
    public string LearningMode { get; set; } = Domain.Commons.Enums.LearningMode.Offline.ToString();
    public string SubjectName { get; set; } = string.Empty;
}