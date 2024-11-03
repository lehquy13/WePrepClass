using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Infrastructure.Persistence.EntityFrameworkCore.Configs;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName(nameof(User.Id))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value)
            );

        builder.Property(r => r.FirstName)
            .IsRequired();

        builder.Property(r => r.LastName)
            .IsRequired();

        builder.HasIndex(r => r.Email)
            .IsUnique();

        builder.Property(r => r.PhoneNumber);

        builder.Property(r => r.Gender)
            .IsRequired();

        builder.Property(r => r.BirthYear)
            .IsRequired();

        builder.Property(r => r.Avatar)
            .IsRequired();

        builder.Property(r => r.Description)
            .IsRequired();

        builder.Property(r => r.Role)
            .IsRequired();

        builder.OwnsOne(user => user.Address,
            navigationBuilder =>
            {
                navigationBuilder.Property(address => address.District)
                    .HasColumnName(nameof(Address.District));
                navigationBuilder.Property(address => address.City)
                    .HasColumnName(nameof(Address.City));
                navigationBuilder.Property(address => address.DetailAddress)
                    .HasColumnName(nameof(Address.DetailAddress));
            });
    }
}