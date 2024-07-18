using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Persistence.Middleware;

namespace WePrepClass.Persistence.EntityFrameworkCore;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IHttpContextAccessor? httpContextAccessor = null
) : DbContext(options)
{
    public DbSet<User> Users { get; init; } = null!;
    public DbSet<Notification> Notifications { get; init; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.EnableSensitiveDataLogging();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(x => x)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        if (httpContextAccessor?.HttpContext is null) return result;

        var domainEventsQueue =
            httpContextAccessor.HttpContext.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey,
                out var value)
            && value is Queue<IDomainEvent> existingDomainEvents
                ? existingDomainEvents
                : new Queue<IDomainEvent>();

        domainEvents.ForEach(domainEventsQueue.Enqueue);
        httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;

        return result;
    }
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB; Database=ca_dev_1; Trusted_Connection=True;MultipleActiveResultSets=true"
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}