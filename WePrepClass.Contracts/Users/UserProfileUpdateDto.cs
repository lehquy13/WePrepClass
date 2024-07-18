using FluentValidation;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Contracts.Users;

public class UserProfileUpdateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public int BirthYear { get; set; } = 1960;
    public string Avatar { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class UserProfileCreateUpdateDtoValidator : AbstractValidator<UserProfileUpdateDto>
{
    public UserProfileCreateUpdateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .WithMessage("First name must be between 2 and 50 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .WithMessage("Last name must be between 2 and 50 characters long.");

        RuleFor(x => x.BirthYear)
            .InclusiveBetween(1900, DateTime.Now.Year)
            .WithMessage("Birth year must be between 1900 and the current year.");

        // Optional validation for Avatar URL (you can adjust based on your needs)
        RuleFor(x => x.Avatar)
            .NotEmpty()
            .WithMessage("Avatar URL must not be empty.");

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("City must be between 1 and 50 characters long.");

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Country must be between 1 and 50 characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must be less than 500 characters long.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d+$") // Matches a sequence of digits
            .WithMessage("Phone number must only contain digits.");
    }
}