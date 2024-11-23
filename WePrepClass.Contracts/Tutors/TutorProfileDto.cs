using FluentValidation;

namespace WePrepClass.Contracts.Tutors;

public class TutorProfileDto
{
    public Guid Id { get; set; }
    public string AcademicLevel { get; set; } = Domain.Commons.Enums.AcademicLevel.UnderGraduate.ToString();
    public string University { get; set; } = string.Empty;
    public IEnumerable<int> MajorIds { get; set; } = [];
    public bool IsVerified { get; set; } = false;
}

public class TutorBasicUpdateDtoValidator : AbstractValidator<TutorProfileDto>
{
    public TutorBasicUpdateDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .NotEmpty().WithMessage("Tutor ID is required.");

        RuleFor(dto => dto.AcademicLevel)
            .NotEmpty().WithMessage("Academic level is required.")
            .MaximumLength(100).WithMessage("Academic level must not exceed 100 characters.");

        RuleFor(dto => dto.University)
            .NotEmpty().WithMessage("University is required.");
    }
}