using Matt.SharedKernel.Domain.Interfaces.Repositories;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Users;

public interface IUserRepository : IRepository
{
    Task<User?> GetByCustomerIdAsync(UserId userId, CancellationToken cancellationToken);
    Task InsertAsync(User user, CancellationToken cancellationToken);
    Task<List<User>> GetListAsync(CancellationToken cancellationToken);
}