using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IQueryRequest<UserDetailDto>, IAuthorizationRequest;

public class GetUserByIdQueryHandler(
    IUserRepository userRepository,
    IAppLogger<GetUserByIdQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetUserByIdQuery, UserDetailDto>(logger, mapper)
{
    public override async Task<Result<UserDetailDto>> Handle(GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByCustomerIdAsync(UserId.Create(request.Id), cancellationToken);

        if (user is null)
        {
            return Result.Fail(UserAppServiceError.UserNotFound);
        }

        var userDetailDto = new UserDetailDto(
            user.Id.Value.ToString(),
            user.GetFullName(),
            user.Email,
            user.Gender.ToString(),
            user.BirthYear);

        return userDetailDto;
    }
}