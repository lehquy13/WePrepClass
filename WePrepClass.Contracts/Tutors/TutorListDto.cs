using Matt.SharedKernel.Application.Contracts.Primitives;

namespace WePrepClass.Contracts.Tutors;

public class TutorListDto : EntityDto<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int BirthYear { get; set; } = 1960;
    public string Description { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string AcademicLevel { get; set; } = Domain.Commons.Enums.AcademicLevel.UnderGraduate.ToString();
    public string University { get; set; } = string.Empty;
    public decimal? Rate { get; set; }
}