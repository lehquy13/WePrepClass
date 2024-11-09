using WePrepClass.Contracts;

namespace WePrepClass.Application.UseCases.Wpc.TutorProfiles;

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

public class VerificationDto : BasicAuditedEntityDto<Guid>
{
    public string Image { get; init; } = "doc_contract.png";
}

public class BasicCourseRequestDto : BasicAuditedEntityDto<Guid>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string RequestStatus { get; set; } = "Pending";
}

public class ChangeVerificationRequestDto 
{
    public Guid Id { get; set; }
    public string RequestStatus { get; set; } = null!;
    public List<string> ChangeVerificationRequestDetails { get; set; } = null!;
}

public class TutorMajorDto
{
    public bool IsMajored { get;  set; }
    public int SubjectId { get;  set; }
    public string SubjectName { get;  set; } = null!;
}
