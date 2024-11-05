using FluentValidation;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Contracts.Courses;

public class CourseCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public LearningMode LearningModeRequirement { get; set; }
    public decimal Fee { get; set; }
    public GenderOption GenderRequirement { get; set; }
    public AcademicLevelOption AcademicLevelRequirement { get; set; }
    public Gender LearnerGender { get; set; }
    public string LearnerName { get; set; } = string.Empty;
    public int NumberOfLearner { get; set; } = 1;
    public string ContactNumber { get; set; } = string.Empty;
    public int MinutePerSession { get; set; } = 90;
    public int SessionPerWeek { get; set; } = 2;
    public string Address { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public DurationUnit SessionDuration { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public class CourseForLearnerCreateDtoValidator : AbstractValidator<CourseCreateDto>
{
    public CourseForLearnerCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(100)
            .WithMessage("Title must be less than 100 characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must be less than 500 characters long.");

        RuleFor(x => x.LearningModeRequirement)
            .NotEmpty()
            .IsInEnum()
            .WithMessage("Learning mode must be a valid option.");

        RuleFor(x => x.Fee)
            .InclusiveBetween(0, decimal.MaxValue)
            .WithMessage("Fee must be a non-negative number.");

        RuleFor(x => x.GenderRequirement)
            .NotEmpty()
            .IsInEnum()
            .WithMessage("Gender requirement must be a valid option.");

        RuleFor(x => x.AcademicLevelRequirement)
            .NotEmpty()
            .IsInEnum()
            .WithMessage("Academic level requirement must be a valid option.");

        RuleFor(x => x.LearnerGender)
            .NotEmpty()
            .IsInEnum()
            .WithMessage("Learner gender must be a valid option.");

        RuleFor(x => x.LearnerName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Learner name must be less than 100 characters long.");

        RuleFor(x => x.NumberOfLearner)
            .InclusiveBetween(1, 100)
            .WithMessage("Number of learners must be between 1 and 100.");

        RuleFor(x => x.ContactNumber)
            .Matches(@"^\d{10}$") // Assuming 10-digit phone number
            .WithMessage("Contact number must be 10 digits long.");

        RuleFor(x => x.MinutePerSession)
            .InclusiveBetween(1, 180)
            .WithMessage("Minutes per session must be between 1 and 180.");

        RuleFor(x => x.SessionPerWeek)
            .InclusiveBetween(1, 7)
            .WithMessage("Sessions per week must be between 1 and 7.");

        RuleFor(x => x.Address)
            .MaximumLength(255)
            .WithMessage("Address must be less than 255 characters long.");

        RuleFor(x => x.SubjectId)
            .GreaterThan(0)
            .WithMessage("Subject ID must be a positive number.");
    }
}