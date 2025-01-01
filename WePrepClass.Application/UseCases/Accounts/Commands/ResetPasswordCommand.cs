using FluentValidation;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Results;
using Microsoft.Extensions.Logging;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Application.UseCases.Accounts.Commands;

public record ResetPasswordCommand(
    string Email,
    string Otp,
    string NewPassword
) : ICommandRequest;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Otp)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(6)
            .WithMessage("OTP must be 6 digits long.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage(
                "New password must be at least 6 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.");
    }
}

public class ResetPasswordCommandHandler(
    IIdentityService identityService,
    IUnitOfWork unitOfWork,
    ILogger<ResetPasswordCommandHandler> logger
) : CommandHandlerBase<ResetPasswordCommand>(unitOfWork)
{
    public override async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService
            .ResetPasswordAsync(
                command.Email,
                command.NewPassword,
                command.Otp);

        if (result.IsSuccess && await UnitOfWork.SaveChangesAsync(cancellationToken) > 0) return Result.Success();

        logger.LogError("Reset password fail: {Message}", result.Error.ToString());

        return AuthenticationErrorConstants.ResetPasswordFail;
    }
}