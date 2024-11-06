namespace WePrepClass.Contracts.Tutors;

public class TutorEnrollmentDto
{
    public string University { get; set; } = string.Empty;
    public string AcademicLevel { get; set; } = string.Empty;
    public List<int> MajorIds { get; set; } = [];
}