﻿using FluentValidation;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Application.UseCases.Accounts;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Users.Commands;

public record CreateUserCommand(
    string Username,
    string Email,
    string PhoneNumber,
    string Password,
    string FirstName,
    string LastName,
    int BirthYear,
    string City,
    string Country,
    Gender Gender
) : ICommandRequest;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9_]+$") // Only letters, numbers, and underscores
            .WithMessage(
                "Username must be between 3 and 50 characters long, and can only contain letters, numbers, and underscores.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d+$") // Matches a sequence of digits
            .WithMessage("Phone number must only contain digits.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage(
                "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.");
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(50)
            .WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(50)
            .WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.BirthYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year)
            .WithMessage("Invalid birth year.");
    }
}

public class CreateUserCommandHandler(
    IIdentityService identityService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IAppLogger<CreateUserCommandHandler> logger
) : CommandHandlerBase<CreateUserCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.CreateAsync(
            command.Username,
            command.FirstName,
            command.LastName,
            command.Gender,
            command.BirthYear,
            Address.Create(command.City, command.Country),
            command.Country,
            string.Empty,
            command.Email,
            command.PhoneNumber);

        if (!result.IsSuccess) return Result.Fail(result.Error);

        await userRepository.InsertAsync(result.Value, cancellationToken);

        return await UnitOfWork.SaveChangesAsync(cancellationToken) <= 0
            ? Result.Fail(AuthenticationErrorConstants.CreateUserFailWhileSavingChanges)
            : Result.Success();
    }
}