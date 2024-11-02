using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Infrastructure.Persistence.EntityFrameworkCore;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>(builder =>
        {
            var id = 1;

            foreach (var role in EnumProvider.Roles)
            {
                builder.HasData(
                    new IdentityRole
                    {
                        Id = id++.ToString(),
                        Name = role,
                        NormalizedName = role.ToUpper()
                    }
                );
            }
        });
    }
}

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB; Database=ca_dev_identity_1; Trusted_Connection=True;MultipleActiveResultSets=true"
        );

        return new IdentityDbContext(optionsBuilder.Options);
    }
}