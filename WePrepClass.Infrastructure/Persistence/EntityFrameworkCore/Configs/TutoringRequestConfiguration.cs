using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePrepClass.Domain.WePrepClassAggregates.TutoringRequests;
using WePrepClass.Domain.WePrepClassAggregates.TutoringRequests.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Infrastructure.Persistence.EntityFrameworkCore.Configs;

public class TutoringRequestConfiguration : IEntityTypeConfiguration<TutoringRequest>
{
    public void Configure(EntityTypeBuilder<TutoringRequest> builder)
    {
        builder.ToTable(nameof(TutoringRequest));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName(nameof(TutoringRequest.Id))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => TutorRequestId.Create(value)
            );

        builder.Property(r => r.TutorId)
            .HasColumnName(nameof(TutoringRequest.TutorId))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => TutorId.Create(value)
            );

        builder.Property(r => r.UserId)
            .HasColumnName(nameof(TutoringRequest.UserId))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value)
            );

        builder.Property(r => r.Message)
            .HasColumnName(nameof(TutoringRequest.Message))
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(r => r.TutorRequestStatus)
            .HasColumnName(nameof(TutoringRequest.TutorRequestStatus))
            .IsRequired();

        builder.HasOne<Tutor>()
            .WithMany()
            .HasForeignKey(nameof(TutoringRequest.TutorId))
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(nameof(TutoringRequest.UserId))
            .OnDelete(DeleteBehavior.Restrict);
    }
}