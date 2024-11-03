using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

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
            .HasColumnName("Id")
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

        builder.Property(r => r.TutorId)
            .ValueGeneratedNever()
            .HasConversion(
                id => id!.Value,
                value => TutorId.Create(value)
            );

        builder.HasOne<Tutor>()
            .WithMany()
            .HasForeignKey(nameof(Course.TutorId))
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        //TODO: Create a constant that tutorId must be diff from learnerId

        builder.OwnsOne(course => course.SessionFee, navigationBuilder =>
        {
            navigationBuilder.Property(fee => fee.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("SessionFee");

            navigationBuilder.Property(fee => fee.Currency)
                .HasMaxLength(10)
                .HasColumnName(nameof(Course.SessionFee.Currency));
        });

        builder.OwnsOne(course => course.ChargeFee, navigationBuilder =>
        {
            navigationBuilder.Property(fee => fee.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("ChargeFee");

            navigationBuilder.Property(fee => fee.Currency)
                .HasColumnName(nameof(Course.ChargeFee.Currency));
        });

        builder.OwnsOne(o => o.Review, navigationBuilder =>
        {
            navigationBuilder.Property(r => r.Rate)
                .HasColumnName(nameof(Review.Rate))
                .IsRequired();

            navigationBuilder.Property(r => r.Detail)
                .HasColumnName(nameof(Review.Detail))
                .HasMaxLength(Review.MaxDetailLength)
                .IsRequired();

            navigationBuilder
                .Property(r => r.CreationTime)
                .HasColumnName("Review_CreationTime");

            navigationBuilder
                .Property(r => r.CreatorId)
                .HasMaxLength(36)
                .HasColumnName("Review_CreatorId");

            navigationBuilder
                .Property(r => r.LastModificationTime)
                .HasColumnName("Review_LastModificationTime");

            navigationBuilder
                .Property(r => r.LastModifierId)
                .HasMaxLength(36)
                .HasColumnName("Review_LastModifierId");
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