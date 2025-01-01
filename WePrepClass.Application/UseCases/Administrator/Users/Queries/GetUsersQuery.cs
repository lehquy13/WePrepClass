using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Paginations;
using Matt.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Users;

namespace WePrepClass.Application.UseCases.Administrator.Users.Queries;

public record GetUsersQuery(int PageIndex, int PageSize = 10)
    : IQueryRequest<PaginatedList<UserDto>>, IAuthorizationRequired;

public class GetUsersQueryHandler(
    IReadDbContext userRepository
) : QueryHandlerBase<GetUsersQuery, PaginatedList<UserDto>>
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