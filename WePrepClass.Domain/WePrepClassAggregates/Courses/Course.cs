using Matt.SharedKernel.Domain;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Primitives.Auditing;
using Matt.SharedKernel.Results;
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
    public const int MaxTitleLength = 256;
    public const int MaxDescriptionLength = 512;
    public const int MaxNoteLength = 256;

    private readonly List<TeachingAssignment> _teachingAssignments = [];
    private readonly List<TeachingRequest> _teachingRequests = [];

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Note { get; private set; } = null!;
    public CourseStatus Status { get; set; } = CourseStatus.PendingApproval;
    public LearningMode LearningModeRequirement { get; private set; } = LearningMode.Offline;
    public Fee SessionFee { get; private set; } = Fee.Create(0, CurrencyCode.Vnd);
    public Fee ChargeFee { get; private set; } = Fee.Create(0, CurrencyCode.Vnd);
    public Session Session { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public Review? Review { get; private set; }
    private DateTime? ConfirmedDate { get; set; }

    public TutorSpecification TutorSpecification { get; private set; } = null!;
    public LearnerDetail LearnerDetail { get; private set; } = null!;

    public SubjectId SubjectId { get; private set; } = null!;

    public IReadOnlyList<TeachingRequest> TeachingRequests => _teachingRequests.AsReadOnly();
    public IReadOnlyList<TeachingAssignment> TeachingAssignments => _teachingAssignments.AsReadOnly();

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
        if (title.Length is < MinTitleLength or > MaxTitleLength)
        {
            return DomainErrors.Courses.TitleLengthOutOfRange;
        }

        if (description.Length > MaxDescriptionLength)
        {
            return DomainErrors.Courses.DescriptionLengthOutOfRange;
        }

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

        course.DomainEvents.Add(new CourseCreatedCourseDomainEvent(course));

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
        if (title.Length is < MinTitleLength or > MaxTitleLength)
        {
            return DomainErrors.Courses.TitleLengthOutOfRange;
        }

        if (description.Length > MaxDescriptionLength)
        {
            return DomainErrors.Courses.DescriptionLengthOutOfRange;
        }

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

    public Result ReviewCourse(short rate, string detail, string reviewer)
    {
        if (Status is not CourseStatus.Confirmed)
        {
            return DomainErrors.Courses.NotBeenConfirmed;
        }

        if (ConfirmedDate?.AddDays(30) > DateTimeProvider.Now) return DomainErrors.Courses.ReviewNotAllowedYet;

        var result = Review.Create(rate, detail, reviewer);

        if (result.IsFailed)
        {
            return result.Error;
        }

        Review = result.Value;

        DomainEvents.Add(new CourseReviewedDomainEvent(this));

        return Result.Success();
    }

    public Result AssignTutor(TutorId tutorId)
    {
        if (LearnerDetail.LearnerId != null && LearnerDetail.LearnerId.Value == tutorId.Value)
        {
            return DomainErrors.Courses.TutorAndLearnerShouldNotBeTheSame;
        }

        if (_teachingAssignments.Find(x => x.TutorId == tutorId) is { } teachingAssignment)
        {
            return teachingAssignment.TeachingAssignmentStatus switch
            {
                TeachingAssignmentStatus.Assigned => DomainErrors.Courses.TutorHadBeenAssigned,
                _ => DomainErrors.Courses.CannotAssignedThisTutorAfterDisassociating
            };
        }

        foreach (var teachingRequest in _teachingRequests)
        {
            if (teachingRequest.TutorId == tutorId &&
                teachingRequest.TeachingRequestStatus == RequestStatus.InProgress)
            {
                teachingRequest.Approve();

                continue;
            }

            teachingRequest.Cancel();
        }

        Status = CourseStatus.InProgress;

        _teachingAssignments.Add(TeachingAssignment.Create(tutorId, Id));

        DomainEvents.Add(new TutorAssignedDomainEvent(this));

        return Result.Success();
    }

    public void SetCourseStatus(CourseStatus status) => Status = status;

    public Result DissociateTutor(string note)
    {
        if ((short)Status < 3) return DomainErrors.Courses.StatusInvalidForUnassignment;

        if (_teachingAssignments.Single(x => x.TeachingAssignmentStatus == TeachingAssignmentStatus.Assigned) is not
            { } teachingAssignment)
        {
            return DomainErrors.Courses.HaveNotBeenAssigned;
        }

        teachingAssignment.Dissociate();

        _teachingRequests.Find(x => x.TutorId == teachingAssignment.TutorId)?.Cancel();

        Note = note;

        DomainEvents.Add(new TutorDissociatedDomainEvent(this));

        return Result.Success();
    }

    public Result ConfirmCourse()
    {
        if (Status is not CourseStatus.InProgress) return DomainErrors.Courses.HaveNotBeenAssigned;

        Status = CourseStatus.Confirmed;
        ConfirmedDate = DateTimeProvider.Now;

        DomainEvents.Add(new CourseConfirmedDomainEvent(this));

        return Result.Success();
    }

    public Result RefundCourse(string commandNote)
    {
        if (Status is not CourseStatus.Confirmed ||
            GetApprovedTeachingAssignment() is not { } approvedTeachingAssignment)
        {
            return DomainErrors.Courses.HaveNotBeenAssigned;
        }

        Note = commandNote;
        Status = CourseStatus.Refunded;

        approvedTeachingAssignment.Dissociate();

        DomainEvents.Add(new CourseWasRefundedDomainEvent(this, approvedTeachingAssignment));

        return Result.Success();
    }

    public Result AddTeachingRequest(TutorId tutorId)
    {
        if (Status is not CourseStatus.Available) return DomainErrors.Courses.Unavailable;

        if (_teachingRequests.Find(x => x.TutorId == tutorId)
            is { TeachingRequestStatus: RequestStatus.InProgress })
        {
            return DomainErrors.Courses.TeachingRequestAlreadyExist;
        }

        _teachingRequests.Add(TeachingRequest.Create(tutorId, Id));

        DomainEvents.Add(new TeachingRequestCreatedDomainEvent(_teachingRequests[^1]));

        return Result.Success();
    }

    public TeachingAssignment? GetApprovedTeachingAssignment()
        => _teachingAssignments.Find(x => x.TeachingAssignmentStatus == TeachingAssignmentStatus.Assigned);
}

// ReSharper disable NotAccessedPositionalProperty.Global
public record CourseRequirementUpdatedDomainEvent(Course Course) : IDomainEvent;

public record CourseCreatedCourseDomainEvent(Course Course) : IDomainEvent;

public record CourseWasRefundedDomainEvent(Course Course, TeachingAssignment ApprovedTeachingAssignment) : IDomainEvent;

public record CourseConfirmedDomainEvent(Course Course) : IDomainEvent;

public record CourseReviewedDomainEvent(Course Course) : IDomainEvent;

public record TutorAssignedDomainEvent(Course Course) : IDomainEvent;

public record TutorDissociatedDomainEvent(Course Course) : IDomainEvent;

public record TeachingRequestCreatedDomainEvent(TeachingRequest TeachingRequest) : IDomainEvent;