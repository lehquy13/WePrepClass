using Microsoft.EntityFrameworkCore;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;
using WePrepClass.Persistence.EntityFrameworkCore;

namespace WePrepClass.Persistence.Persistence.Repositories;

public class UserRepository(AppDbContext appDbContext) : IUserRepository
{
    public async Task<User?> GetByCustomerIdAsync(UserId userId, CancellationToken cancellationToken)
        => await appDbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public async Task InsertAsync(User user, CancellationToken cancellationToken) =>
        await appDbContext.Users.AddAsync(user, cancellationToken);

    public async Task<List<User>> GetListAsync(CancellationToken cancellationToken)
        => await appDbContext.Users.ToListAsync(cancellationToken);
}