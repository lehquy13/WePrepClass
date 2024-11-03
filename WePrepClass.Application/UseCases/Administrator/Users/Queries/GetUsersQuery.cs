using MapsterMapper;
using Matt.Paginated;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Users;

namespace WePrepClass.Application.UseCases.Administrator.Users.Queries;

public record GetUsersQuery(int PageIndex, int PageSize = 10)
    : IQueryRequest<PaginatedList<UserDto>>, IAuthorizationRequired;

public class GetUsersQueryHandler(
    IReadDbContext userRepository,
    IAppLogger<GetUsersQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetUsersQuery, PaginatedList<UserDto>>(logger, mapper)
{
    public override async Task<Result<PaginatedList<UserDto>>> Handle(
        GetUsersQuery getUsersQuery,
        CancellationToken cancellationToken)
    {
        var totalUsers = await userRepository.Users.CountAsync(cancellationToken);

        var users = await userRepository.Users
            .Skip((getUsersQuery.PageIndex - 1) * getUsersQuery.PageSize)
            .Take(getUsersQuery.PageSize)
            .ToListAsync(cancellationToken);

        var userDtos = users.Select(x => new UserDto(x.Id.Value, x.GetFullName(), x.Email)).ToList();

        var paginatedList = PaginatedList<UserDto>.Create(
            userDtos,
            getUsersQuery.PageIndex,
            getUsersQuery.PageSize,
            totalUsers);

        return paginatedList;
    }
}