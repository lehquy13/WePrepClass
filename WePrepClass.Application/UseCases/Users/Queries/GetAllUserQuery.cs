using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Application.UseCases.Users.Queries;

public record GetAllUserQuery : IQueryRequest<IEnumerable<UserDto>>, IAuthorizationRequest;

public class GetAllUserQueryHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUserService,
    IAppLogger<GetAllUserQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetAllUserQuery, IEnumerable<UserDto>>(logger, mapper)
{
    public override async Task<Result<IEnumerable<UserDto>>> Handle(GetAllUserQuery request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return Result<IEnumerable<UserDto>>.Fail("You must be authenticated to perform this action.");
        }

        var users = await userRepository.GetListAsync(cancellationToken);
        var userDtos = users.Select(x => new UserDto(x.Id.Value, x.GetFullName(), x.Email));

        return Result<IEnumerable<UserDto>>.Success(userDtos);
    }
}