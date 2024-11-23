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
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Email)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(r => r.Email)
            .IsUnique();

        builder.Property(r => r.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(r => r.Gender)
            .IsRequired();

        builder.Property(r => r.BirthYear)
            .IsRequired();

        builder.Property(r => r.Avatar)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(r => r.Role)
            .IsRequired();

        builder.OwnsOne(user => user.Address, navigationBuilder =>
        {
            navigationBuilder.Property(address => address.District)
                .HasMaxLength(50)
                .HasColumnName(nameof(Address.District));

            navigationBuilder.Property(address => address.City)
                .HasMaxLength(50)
                .HasColumnName(nameof(Address.City));

            navigationBuilder.Property(address => address.DetailAddress)
                .HasMaxLength(100)
                .HasColumnName(nameof(Address.DetailAddress));
        });
    }
}