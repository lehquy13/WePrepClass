using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Contracts.Users;

public class AttendedCourseDetailDto : BasicAuditedEntityDto<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = CourseStatus.PendingApproval.ToString();
    public string LearningMode { get; set; } = Domain.Commons.Enums.LearningMode.Online.ToString();
    public string SectionFee { get; set; } = string.Empty;
    public decimal ChargeFee { get; set; }
    public int SessionDurationDisplay { get; set; } = 90;
    public string Address { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;

    public string? Detail { get; set; }
    public decimal? Rate { get; set; }

    public Guid TutorId { get; set; }
    public string TutorName { get; set; } = string.Empty;
    public string TutorContact { get; set; } = string.Empty;
    public string TutorEmail { get; set; } = string.Empty;
}