using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Infrastructure.Persistence.EntityFrameworkCore.Configs;

internal class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        ConfigureCourse(builder);
    }

    private static void ConfigureCourse(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable(nameof(Course));

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName(nameof(Course.Id))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => CourseId.Create(value)
            );

        builder.Property(r => r.Title)
            .HasMaxLength(Course.MaxTitleLength)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(Course.MaxDescriptionLength)
            .IsRequired();

        builder.Property(r => r.Note)
            .HasMaxLength(Course.MaxNoteLength)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.LearningModeRequirement)
            .IsRequired();

        builder.OwnsOne(course => course.Session, navigationBuilder =>
        {
            navigationBuilder.Property(session => session.Value)
                .HasPrecision(2, 2)
                .HasColumnName("SessionValue");
            navigationBuilder.Property(session => session.DurationUnit)
                .HasColumnName("SessionDurationUnit");
            navigationBuilder.Property(session => session.SessionFrequency)
                .HasColumnName("SessionFrequency");
        });

        builder.OwnsOne(course => course.Address, navigationBuilder =>
        {
            navigationBuilder.Property(address => address.City)
                .HasColumnName("City");
            navigationBuilder.Property(address => address.District)
                .HasColumnName("District");
            navigationBuilder.Property(address => address.DetailAddress)
                .HasColumnName("DetailAddress");
        });

        builder.Property(r => r.SubjectId)
            .HasColumnName(nameof(Course.SubjectId))
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => SubjectId.Create(value)
            );

        builder.HasOne<Subject>()
            .WithMany()
            .HasForeignKey(nameof(Course.SubjectId))
            .IsRequired();

        builder.OwnsMany(course => course.TeachingAssignments, navigationBuilder =>
        {
            navigationBuilder.ToTable(nameof(TeachingAssignment));

            navigationBuilder.HasKey(assignment => assignment.Id);
            navigationBuilder.Property(assignment => assignment.Id)
                .HasColumnName(nameof(TeachingAssignment.Id))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => TeachingAssignmentId.Create(value)
                );

            navigationBuilder.Property(assignment => assignment.TeachingAssignmentStatus)
                .HasColumnName(nameof(TeachingAssignment.TeachingAssignmentStatus))
                .IsRequired();

            navigationBuilder.Property(assignment => assignment.CourseId)
                .HasColumnName(nameof(TeachingAssignment.CourseId))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => CourseId.Create(value)
                );

            navigationBuilder.Property(assignment => assignment.TutorId)
                .HasColumnName(nameof(TeachingAssignment.TutorId))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => TutorId.Create(value)
                );
        });

        builder.OwnsMany(course => course.TeachingRequests, navigationBuilder =>
        {
            navigationBuilder.ToTable(nameof(TeachingRequest));

            navigationBuilder.HasKey(request => request.Id);
            navigationBuilder.Property(request => request.Id)
                .HasColumnName(nameof(TeachingRequest.Id))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => TeachingRequestId.Create(value)
                );

            navigationBuilder.Property(request => request.TeachingRequestStatus)
                .HasColumnName(nameof(TeachingRequest.TeachingRequestStatus))
                .IsRequired();

            navigationBuilder.Property(request => request.Description)
                .HasColumnName("TeachingRequest_Description")
                .HasMaxLength(256);

            navigationBuilder.Property(tr => tr.TutorId)
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

            navigationBuilder.Property(tr => tr.CourseId)
                .HasColumnName(nameof(TeachingRequest.CourseId))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => CourseId.Create(value)
                );
        });

        builder.OwnsOne(course => course.SessionFee, navigationBuilder =>
        {
            navigationBuilder.Property(fee => fee.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("SessionFee");

            navigationBuilder.Property(fee => fee.Currency)
                .HasMaxLength(10)
                .HasColumnName($"{nameof(Course.SessionFee)}.{nameof(Course.SessionFee.Currency)}");
        });

        builder.OwnsOne(course => course.ChargeFee, navigationBuilder =>
        {
            navigationBuilder.Property(fee => fee.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("ChargeFee");

            navigationBuilder.Property(fee => fee.Currency)
                .HasColumnName($"{nameof(Course.ChargeFee)}.{nameof(Course.ChargeFee.Currency)}");
        });

        builder.OwnsOne(o => o.Review, navigationBuilder =>
        {
            navigationBuilder.Property(r => r.Rate)
                .HasColumnName(nameof(Review.Rate))
                .HasPrecision(2, 1)
                .IsRequired();

            navigationBuilder.Property(r => r.Detail)
                .HasColumnName(nameof(Review.Detail))
                .HasMaxLength(Review.MaxDetailLength)
                .IsRequired();

            navigationBuilder
                .Property(r => r.CreationTime)
                .HasColumnName($"{nameof(Review)}_{nameof(Review.CreationTime)}");

            navigationBuilder
                .Property(r => r.CreatorId)
                .HasMaxLength(36)
                .HasColumnName($"{nameof(Review)}_{nameof(Review.CreatorId)}");

            navigationBuilder
                .Property(r => r.LastModificationTime)
                .HasColumnName($"{nameof(Review)}_{nameof(Review.LastModificationTime)}");

            navigationBuilder
                .Property(r => r.LastModifierId)
                .HasMaxLength(36)
                .HasColumnName($"{nameof(Review)}_{nameof(Review.LastModifierId)}");
        });

        builder.OwnsOne(o => o.LearnerDetail, navigationBuilder =>
        {
            navigationBuilder.Property(r => r.LearnerGender)
                .HasColumnName(nameof(LearnerDetail.LearnerGender))
                .IsRequired();

            navigationBuilder.Property(r => r.LearnerName)
                .HasColumnName(nameof(LearnerDetail.LearnerName))
                .HasMaxLength(100)
                .IsRequired();

            navigationBuilder.Property(r => r.NumberOfLearner)
                .HasColumnName(nameof(LearnerDetail.NumberOfLearner))
                .IsRequired();

            navigationBuilder.Property(r => r.ContactNumber)
                .HasColumnName(nameof(LearnerDetail.ContactNumber))
                .HasMaxLength(20)
                .IsRequired();

            navigationBuilder.Property(r => r.LearnerId)
                .HasColumnName(nameof(LearnerDetail.LearnerId))
                .ValueGeneratedNever()
                .HasConversion(
                    id => id!.Value,
                    value => UserId.Create(value)
                );

            navigationBuilder.HasOne<User>()
                .WithMany()
                .HasForeignKey(nameof(Course.LearnerDetail.LearnerId))
                .IsRequired(false);
        });

        builder.OwnsOne(o => o.TutorSpecification, navigationBuilder =>
        {
            navigationBuilder.Property(r => r.TutorGender)
                .HasColumnName(nameof(TutorSpecification.TutorGender))
                .IsRequired();

            navigationBuilder.Property(r => r.TutorAcademicLevel)
                .HasColumnName(nameof(TutorSpecification.TutorAcademicLevel))
                .IsRequired();
        });
    }
}