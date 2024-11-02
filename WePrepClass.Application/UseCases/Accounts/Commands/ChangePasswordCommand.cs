using FluentValidation;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Accounts.Commands;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmedPassword
) : ICommandRequest, IAuthorizationRequired;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Current password must be at least 6 characters long.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage(
                "New password must be at least 6 characters long and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.");

        RuleFor(x => x.ConfirmedPassword)
            .NotEmpty()
            .Equal(x => x.NewPassword)
            .WithMessage("Confirmed password must match the new password.");
    }
}

public class ChangePasswordCommandHandler(
    IIdentityService identityService,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IAppLogger<ChangePasswordCommandHandler> logger
) : CommandHandlerBase<ChangePasswordCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService
            .ChangePasswordAsync(
                UserId.Create(currentUserService.UserId),
                command.CurrentPassword,
                command.NewPassword);

        if (result.IsSuccess && await UnitOfWork.SaveChangesAsync(cancellationToken) > 0) return Result.Success();

        Logger.LogWarning("Change password fail", result.Error.ToString());
        return Result.Fail(AuthenticationErrorConstants.ChangePasswordFail);
    }
}