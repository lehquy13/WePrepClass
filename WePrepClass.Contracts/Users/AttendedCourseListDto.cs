namespace WePrepClass.Contracts.Users;

public class AttendedCourseListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}