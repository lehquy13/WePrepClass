using FluentValidation;
using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Application.Interfaces;

namespace WePrepClass.Application.UseCases.Accounts.Queries;

public record ValidateTokenQuery(string ValidateToken) : IQueryRequest;

public class ValidateTokenQueryValidator : AbstractValidator<ValidateTokenQuery>
{
    public ValidateTokenQueryValidator()
    {
        RuleFor(x => x.ValidateToken)
            .NotEmpty()
            .WithMessage("Please enter a valid token.");
    }
}

public class ValidateTokenQueryHandler(
    IAppLogger<ValidateTokenQueryHandler> logger,
    IMapper mapper,
    IJwtTokenGenerator jwtTokenGenerator
) : QueryHandlerBase<ValidateTokenQuery>(logger, mapper)
{
    public override Task<Result> Handle(ValidateTokenQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(jwtTokenGenerator.ValidateToken(request.ValidateToken).Value.Any()
            ? Result.Success()
            : Result.Fail("Token is invalid."));
}