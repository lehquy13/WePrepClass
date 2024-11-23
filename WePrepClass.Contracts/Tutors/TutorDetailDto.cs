using WePrepClass.Contracts.Users;

namespace WePrepClass.Contracts.Tutors;

public class TutorDetailDto : UserProfileDto
{
    public required string AcademicLevel { get; init; } = Domain.Commons.Enums.AcademicLevel.UnderGraduate.ToString();
    public required string University { get; init; } = string.Empty;
    public required bool IsVerified { get; init; }
    public required List<MajorDto> Majors { get; init; } = new();
    public required List<VerificationDto> Verifications { get; init; } = [];
    public required List<ChangeVerificationRequestDto> VerificationChanges { get; init; } = [];
}

public class ChangeVerificationRequestDto
{
    public required Guid Id { get; set; }
    public required string RequestStatus { get; set; } = null!;
    public required List<string> ChangeVerificationRequestDetails { get; set; } = null!;
}

public class VerificationDto
{
    public required string Image { get; init; } = "doc_contract.png";
}

public class MajorDto
{
    public required string Name { get; init; } = string.Empty;
    public required bool IsSelected { get; init; }
}

public class ReviewDetailDto
{
    public required Guid CourseId { get; set; }
    public required Guid LearnerId { get; set; }
    public required string LearnerName { get; set; } = string.Empty;
    public required short Rate { get; set; } = 5;
    public required string Detail { get; set; } = string.Empty;
}