using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Accounts.Queries;

public record GetUserProfileQuery : IQueryRequest<UserProfileDto>, IAuthorizationRequired;

public class GetUserProfileQueryHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUserService,
    IMapper mapper,
    IAppLogger<GetUserProfileQueryHandler> logger
) : QueryHandlerBase<GetUserProfileQuery, UserProfileDto>(logger, mapper)
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
            : Mapper.Map<UserProfileDto>(customer);
    }
}