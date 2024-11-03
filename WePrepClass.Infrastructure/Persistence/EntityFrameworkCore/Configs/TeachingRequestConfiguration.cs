using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.TeachingRequests;
using WePrepClass.Domain.WePrepClassAggregates.TeachingRequests.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Infrastructure.Persistence.EntityFrameworkCore.Configs;

internal class TeachingRequestConfiguration : IEntityTypeConfiguration<TeachingRequest>
{
    public void Configure(EntityTypeBuilder<TeachingRequest> builder)
    {
        builder.ToTable(nameof(TeachingRequest));

        builder.HasKey(tr => tr.Id);
        builder.Property(tr => tr.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => TeachingRequestId.Create(value)
            );

        builder.Property(tr => tr.Description)
            .IsRequired();

        builder.Property(tr => tr.TeachingRequestStatus)
            .IsRequired();

        builder.Property(tr => tr.TutorId)
            .HasColumnName(nameof(TeachingRequest.TutorId))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => TutorId.Create(value)
            );

        builder.HasOne<Tutor>()
            .WithMany()
            .HasForeignKey(nameof(TeachingRequest.TutorId))
            .IsRequired();

        builder.Property(tr => tr.CourseId)
            .HasColumnName(nameof(TeachingRequest.CourseId))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => CourseId.Create(value)
            );

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(nameof(TeachingRequest.CourseId))
            .IsRequired();
    }
}