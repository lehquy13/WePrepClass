using MapsterMapper;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Results;
using WePrepClass.Contracts.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Accounts.Queries;

public record GetUserProfileQuery : IQueryRequest<UserProfileDto>, IAuthorizationRequired;

public class GetUserProfileQueryHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUserService,
    IMapper mapper
) : QueryHandlerBase<GetUserProfileQuery, UserProfileDto>
{
    public override async Task<Result<UserProfileDto>> Handle(
        GetUserProfileQuery getAllUserQuery,
        CancellationToken cancellationToken)
    {
        var customer = await userRepository.GetByCustomerIdAsync(
            UserId.Create(currentUserService.UserId),
            cancellationToken);

        return customer is null
            ? Result.Fail(AccountServiceErrorConstants.NonExistUserError)
            : mapper.Map<UserProfileDto>(customer);
    }
}