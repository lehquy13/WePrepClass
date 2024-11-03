using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Infrastructure.Persistence.EntityFrameworkCore.Configs;

internal class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        ConfigureTutor(builder);
        ConfigureMajor(builder);
        ConfigureVerificationChange(builder);
        ConfigureVerification(builder);
    }

    private static void ConfigureTutor(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable(nameof(Tutor));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName(nameof(Tutor.Id))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => TutorId.Create(value)
            );

        builder.Property(r => r.UserId)
            .HasColumnName(nameof(Tutor.UserId))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value)
            );

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Tutor>(nameof(Tutor.UserId))
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(r => r.AcademicLevel).IsRequired();
        builder.Property(r => r.University).IsRequired();
        builder.Property(r => r.TutorStatus).IsRequired();
        builder.Property(r => r.Rate).IsRequired();

        builder.HasIndex(p => p.AcademicLevel); // Note: disable this index when seeding data
        builder.HasIndex(p => p.Rate);
    }

    private static void ConfigureMajor(EntityTypeBuilder<Tutor> builder)
    {
        builder.OwnsMany(o => o.Majors, ib =>
        {
            ib.ToTable(nameof(Major));
            ib.HasKey(x => x.Id);

            ib.Property(r => r.Id)
                .HasColumnName(nameof(Major.Id))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => MajorId.Create(value)
                );

            ib.Property(r => r.TutorId)
                .HasColumnName(nameof(Major.TutorId))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => TutorId.Create(value)
                );

            ib.WithOwner().HasForeignKey(x => x.TutorId);

            ib.Property(r => r.SubjectId)
                .HasColumnName(nameof(Major.SubjectId))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => SubjectId.Create(value)
                );

            ib.HasOne<Subject>()
                .WithMany()
                .HasForeignKey(nameof(Major.SubjectId))
                .IsRequired();
        });
    }

    private static void ConfigureVerificationChange(EntityTypeBuilder<Tutor> builder)
    {
        builder.OwnsOne(o => o.VerificationChange, ib =>
        {
            ib.ToTable(nameof(VerificationChange));

            ib.HasKey(x => x.Id);
            ib.Property(r => r.Id)
                .HasColumnName(nameof(VerificationChange.Id))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => VerificationChangeId.Create(value)
                );

            ib.Property(r => r.TutorId)
                .HasColumnName(nameof(VerificationChange.TutorId))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => TutorId.Create(value)
                );

            ib.Property(r => r.VerificationChangeStatus).IsRequired();

            ib.OwnsMany(cvr => cvr.ChangeVerificationRequestDetails, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToTable(nameof(VerificationChangeDetail));

                ownedNavigationBuilder.HasKey(x => x.Id);
                ownedNavigationBuilder.Property(r => r.Id)
                    .HasColumnName(nameof(VerificationChangeDetail.Id))
                    .ValueGeneratedNever()
                    .HasConversion(
                        id => id.Value,
                        value => VerificationChangeDetailId.Create(value)
                    );

                ownedNavigationBuilder.WithOwner().HasForeignKey(x => x.VerificationChangeId);
                ownedNavigationBuilder
                    .Property(r => r.ImageUrl)
                    .HasMaxLength(256)
                    .IsRequired();
            });
        });
    }

    private static void ConfigureVerification(EntityTypeBuilder<Tutor> builder)
    {
        builder.OwnsMany(o => o.Verifications, ib =>
        {
            ib.ToTable(nameof(Verification));
            ib.HasKey(x => x.Id);
            ib.Property(r => r.Id)
                .HasColumnName(nameof(Verification.Id))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => VerificationId.Create(value)
                );
            ib.WithOwner().HasForeignKey(x => x.TutorId);
            ib.Property(x => x.Image).IsRequired();
        });
    }
}