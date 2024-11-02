using Matt.Auditing;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Infrastructure.Persistence.EntityFrameworkCore;

namespace WePrepClass.Infrastructure.Persistence;

internal sealed class UnitOfWork(
    IAppLogger<UnitOfWork> logger,
    AppDbContext appDbContext,
    IdentityDbContext identityDbContext,
    ICurrentUserService currentUserService
) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();

        logger.LogInformation("On save changes...");

        var appSaved = await appDbContext.SaveChangesAsync(cancellationToken);
        var identitySaved = await identityDbContext.SaveChangesAsync(cancellationToken);

        return appSaved + identitySaved;
    }

    private void UpdateAuditableEntities()
    {
        var hasCreationTimeEntries = appDbContext.ChangeTracker.Entries<IHasCreationTime>();

        foreach (var entityEntry in hasCreationTimeEntries)
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(e => e.CreationTime).CurrentValue = DateTime.Now;

                // If the entity is type of ICreationAuditedObject<T>, we should set CreatorId
                if (entityEntry.Entity is ICreationAuditedObject)
                    entityEntry.Property(nameof(ICreationAuditedObject.CreatorId)).CurrentValue =
                        currentUserService.UserId.ToString();
            }

        var hasModificationTimeEntries = appDbContext.ChangeTracker.Entries<IHasModificationTime>();

        foreach (var entityEntry in hasModificationTimeEntries)
        {
            if (entityEntry.State is not (EntityState.Added or EntityState.Modified)) continue;

            entityEntry.Property(e => e.LastModificationTime).CurrentValue = DateTime.Now;

            if (entityEntry.Entity is IModificationAuditedObject)
                entityEntry.Property(nameof(IModificationAuditedObject.LastModifierId)).CurrentValue =
                    currentUserService.UserId.ToString();
        }
    }
}