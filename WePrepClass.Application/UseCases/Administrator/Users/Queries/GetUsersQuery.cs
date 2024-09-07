using MapsterMapper;
using Matt.Paginated;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Application.UseCases.Administrator.Users.Queries;

public record GetUsersQuery(int PageNumber, int PageSize = 10)
    : IQueryRequest<PaginatedList<UserDto>>, IAuthorizationRequest;

public class GetUsersQueryHandler(
    IUserRepository userRepository,
    IAppLogger<GetUsersQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetUsersQuery, PaginatedList<UserDto>>(logger, mapper)
{
    public override async Task<Result<PaginatedList<UserDto>>> Handle(
        GetUsersQuery getUsersQuery,
        CancellationToken cancellationToken)
    {
        var users = await userRepository.GetPaginatedListAsync(getUsersQuery.PageNumber,
            getUsersQuery.PageSize,
            cancellationToken);

        var userDtos = users.Select(x => new UserDto(x.Id.Value, x.GetFullName(), x.Email)).ToList();

        var paginatedList = PaginatedList<UserDto>.Create(userDtos, getUsersQuery.PageNumber,
            getUsersQuery.PageSize,
            users.Count);

        return paginatedList;
    }
}