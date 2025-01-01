namespace WePrepClass.Contracts.Tutors;

public class TutorForProfileDto : BasicAuditedEntityDto<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Role { get; } = "Tutor";
    public string AcademicLevel { get; set; } = "Student";
    public string University { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public decimal? Rate { get; set; }
    public List<TutorMajorDto> Majors { get; set; } = [];
    public List<VerificationDto> VerificationDtos { get; set; } = [];
    public List<ChangeVerificationRequestDto> ChangeVerificationRequestDtos { get; set; } = [];
    public List<BasicCourseRequestDto> BasicCourseRequests { get; set; } = [];
}

public class BasicCourseRequestDto : BasicAuditedEntityDto<Guid>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string RequestStatus { get; set; } = "Pending";
}


public class TutorMajorDto
{
    public bool IsMajored { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = null!;
}