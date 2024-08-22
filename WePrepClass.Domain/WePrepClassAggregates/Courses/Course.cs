using Matt.ResultObject;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses;

public sealed class Course : FullAuditedAggregateRoot<CourseId>
{
    private const int MinTitleLength = 50;

    private readonly List<TeachingRequest> _teachingRequests = [];

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Note { get; private set; } = null!;
    public CourseStatus Status { get; set; } = CourseStatus.PendingApproval;
    public LearningMode LearningModeRequirement { get; private set; } = LearningMode.Offline;
    public Fee SessionFee { get; private set; } = Fee.Create(0, CurrencyCode.VND);
    public Fee ChargeFee { get; private set; } = Fee.Create(0, CurrencyCode.VND);
    public Session Session { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public Review? Review { get; private set; }
    private DateTime? ConfirmedDate { get; set; }

    public TutorSpecification TutorSpecification { get; private set; } = null!;
    public LearnerDetail LearnerDetail { get; private set; } = null!;

    public SubjectId SubjectId { get; private set; } = null!;
    public TutorId? TutorId { get; private set; }

    public IReadOnlyCollection<TeachingRequest> TeachingRequests => _teachingRequests.AsReadOnly();

    private Course()
    {
    }

    public static Result<Course> Create(
        string title,
        string description,
        LearningMode learningMode,
        Fee sessionFee,
        Fee chargeFee,
        LearnerDetail learnerDetail,
        TutorSpecification tutorSpecification,
        Session session,
        Address address,
        SubjectId subjectId)
    {
        if (title.Length < MinTitleLength) return DomainErrors.Courses.CourseTitleTooShort;

        var course = new Course
        {
            Id = CourseId.Create(),
            Title = title,
            Description = description,
            LearningModeRequirement = learningMode,
            SessionFee = sessionFee,
            ChargeFee = chargeFee,
            Session = session,
            Address = address,
            SubjectId = subjectId,
            LearnerDetail = learnerDetail,
            TutorSpecification = tutorSpecification,
            Note = string.Empty
        };

        course.DomainEvents.Add(new NewCourseCreatedCourseEvent(course));

        return course;
    }

    public Result UpdateCourse(
        string title,
        string description,
        LearningMode learningMode,
        Fee sessionFee,
        Fee chargeFee,
        LearnerDetail learnerDetail,
        TutorSpecification tutorSpecification,
        Session session,
        Address address,
        SubjectId subjectId)
    {
        if (title.Length < MinTitleLength) return DomainErrors.Courses.CourseTitleTooShort;

        Title = title;
        Description = description;
        LearningModeRequirement = learningMode;
        SessionFee = sessionFee;
        ChargeFee = chargeFee;
        Session = session;

        LearnerDetail = learnerDetail;
        TutorSpecification = tutorSpecification;

        Address = address;
        SubjectId = subjectId;

        DomainEvents.Add(new CourseRequirementUpdatedDomainEvent(this));

        return Result.Success();
    }

    public Result ReviewCourse(short rate, string detail)
    {
        if (Status is not CourseStatus.Confirmed)
        {
            return DomainErrors.Courses.CourseNotBeenConfirmed;
        }

        if (ConfirmedDate?.AddDays(30) > DateTimeProvider.Now) return DomainErrors.Courses.ReviewNotAllowedYet;

        var result = Review.Create(rate, detail);

        if (result.IsFailed)
        {
            return result.Error;
        }

        Review = result.Value;

        DomainEvents.Add(new CourseReviewedDomainEvent(this));

        return Result.Success();
    }

    public Result AddTeachingRequest(TeachingRequest teachingRequestToCreate)
    {
        if (Status is not CourseStatus.Available) return DomainErrors.Courses.CourseUnavailable;

        if (_teachingRequests.Any(x => x.TutorId == teachingRequestToCreate.TutorId))
            return Result.Fail(DomainErrors.Courses.TeachingRequestAlreadyExist);

        _teachingRequests.Add(teachingRequestToCreate);

        DomainEvents.Add(new CourseRequestedDomainEvent(this));

        return Result.Success();
    }

    public Result AssignTutor(TutorId tutorId)
    {
        if (LearnerDetail.LearnerId?.Value == tutorId.Value)
            return DomainErrors.Courses.TutorAndLearnerShouldNotBeTheSame;

        Status = CourseStatus.InProgress;
        TutorId = tutorId;

        // Check if there is a request exist before, then approve it and cancel the rest
        foreach (var courseRequest in _teachingRequests)
        {
            if (courseRequest.TutorId == tutorId)
            {
                courseRequest.Approved();
            }
            else
            {
                courseRequest.Cancel();
            }
        }

        DomainEvents.Add(new TutorAssignedDomainEvent(this));

        return Result.Success();
    }

    public void SetCourseStatus(CourseStatus status)
    {
        Status = status;
    }

    public Result DissociateTutor(string detailMessage = "")
    {
        if ((short)Status < 3) return DomainErrors.Courses.CourseStatusInvalidForUnassignment;

        // Check if there is a request approved before, then cancel it
        _teachingRequests.FirstOrDefault(x => x.TutorId == TutorId)?.Cancel(detailMessage);

        TutorId = null;

        DomainEvents.Add(new TutorDissociatedDomainEvent(this));

        return Result.Success();
    }

    public Result ConfirmCourse()
    {
        if (Status is not CourseStatus.InProgress || TutorId is null) return DomainErrors.Courses.CourseNotBeenAssigned;

        Status = CourseStatus.Confirmed;
        ConfirmedDate = DateTimeProvider.Now;

        DomainEvents.Add(new CourseConfirmedDomainEvent(this));

        return Result.Success();
    }

    public Result RefundCourse(string commandNote)
    {
        if (Status is not CourseStatus.Confirmed) return DomainErrors.Courses.CourseUnavailable;

        Note = commandNote;
        Status = CourseStatus.Refunded;

        DomainEvents.Add(new CanceledAndRefundedCourseEvent(this));

        return Result.Success();
    }
}

// ReSharper disable NotAccessedPositionalProperty.Global
public record CourseRequirementUpdatedDomainEvent(Course Course) : IDomainEvent;

public record NewCourseCreatedCourseEvent(Course Course) : IDomainEvent;

public record CanceledAndRefundedCourseEvent(Course Course) : IDomainEvent;

public record CourseConfirmedDomainEvent(Course Course) : IDomainEvent;

public record CourseRequestedDomainEvent(Course Course) : IDomainEvent;

public record CourseReviewedDomainEvent(Course Course) : IDomainEvent;

public record TutorAssignedDomainEvent(Course Course) : IDomainEvent;

public record TutorDissociatedDomainEvent(Course Course) : IDomainEvent;