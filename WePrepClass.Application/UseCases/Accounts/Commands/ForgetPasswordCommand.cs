using FluentValidation;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Results;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Application.UseCases.Accounts.Commands;

public record ForgetPasswordCommand(string Email) : ICommandRequest;

public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
{
    public ForgetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Please enter a valid email.");
    }
}

public class ForgetPasswordCommandHandler(
    IIdentityService identityService,
    IUnitOfWork unitOfWork
) : CommandHandlerBase<ForgetPasswordCommand>(unitOfWork)
{
    public override async Task<Result> Handle(ForgetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.ForgetPasswordAsync(command.Email);

        return result.IsFailed || await UnitOfWork.SaveChangesAsync(cancellationToken) <= 0
            ? Result.Fail(AuthenticationErrorConstants.ResetPasswordFail)
            : Result.Success();
    }
}