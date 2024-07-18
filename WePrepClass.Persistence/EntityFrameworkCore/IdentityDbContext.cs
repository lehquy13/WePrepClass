using WePrepClass.Domain.Commons.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WePrepClass.Persistence.EntityFrameworkCore;

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
            builder.HasData(
                [
                    new IdentityRole
                    {
                        Id = "1",
                        Name = Role.BaseUser.ToString(),
                        NormalizedName = Role.BaseUser.ToString().ToUpper()
                    },
                    new IdentityRole
                    {
                        Id = "2",
                        Name = Role.Tenant.ToString(),
                        NormalizedName = Role.Tenant.ToString().ToUpper()
                    },
                    new IdentityRole
                    {
                        Id = "3",
                        Name = Role.Admin.ToString(),
                        NormalizedName = Role.Admin.ToString().ToUpper()
                    }
                ]
            );
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