using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Results;
using WePrepClass.Application.Interfaces;

namespace WePrepClass.Application.UseCases.Accounts.Queries;

public record ValidateTokenQuery(string ValidateToken) : IQueryRequest;

public class ValidateTokenQueryHandler(
    IJwtTokenGenerator jwtTokenGenerator
) : QueryHandlerBase<ValidateTokenQuery>
{
    public override Task<Result> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(jwtTokenGenerator.ValidateToken(request.ValidateToken).Any()
            ? Result.Success()
            : Result.Fail("Token is invalid."));
    }
}