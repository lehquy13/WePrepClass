using Matt.ResultObject;
using Matt.SharedKernel;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

// ReSharper disable PossibleMultipleEnumeration

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors;

public class Tutor : AuditedAggregateRoot<TutorId>
{
    private const int MaxUniversityLength = 100;
    private const int MinUniversityLength = 3;

    private const int MaxRate = 5;
    private const int MinRate = 0;

    private const int MinMajorsCount = 1;
    private const int MaxMajorsCount = 5;

    private List<Verification> _verifications = [];
    private readonly List<SubjectId> _majors = [];

    public UserId UserId { get; private set; } = null!;

    public AcademicLevel AcademicLevel { get; private set; } = AcademicLevel.UnderGraduate;
    public TutorStatus TutorStatus { get; private set; }
    public string University { get; private set; } = null!;
    public decimal? Rate { get; private set; }

    public VerificationChange? VerificationChange { get; private set; }

    public IReadOnlyList<Verification> Verifications => _verifications.AsReadOnly();
    public IReadOnlyList<SubjectId> Majors => _majors.AsReadOnly();

    private Tutor()
    {
    }

    public static Result<Tutor> Create(
        UserId userId,
        AcademicLevel academicLevel,
        string university,
        IList<SubjectId> majors,
        TutorStatus tutorStatus = TutorStatus.Unproven)
    {
        var tutor = new Tutor
        {
            Id = TutorId.Create(userId.Value),
            UserId = userId,
            AcademicLevel = academicLevel,
            TutorStatus = tutorStatus
        };

        var result = DomainValidation.Sequentially(
            () => tutor.SetUniversity(university),
            () => tutor.SetMajors(majors)
        );

        if (result.IsFailed) return result.Error;

        tutor.DomainEvents.Add(new TutorProfileCreatedDomainEvent(tutor));

        return tutor;
    }

    public Result ChangeVerification(List<string> urls)
    {
        var verificationChange = VerificationChange.Create(Id, urls);

        if (verificationChange.IsFailed) return verificationChange.Error;

        if (_verifications.Count == 0)
        {
            _verifications = urls.Select(url => Verification.Create(url, Id)).ToList();
            TutorStatus = TutorStatus.Unproven;
        }
        else
        {
            VerificationChange = verificationChange.Value;
        }

        DomainEvents.Add(new VerificationChangeCreatedDomainEvent(this));

        return Result.Success();
    }

    public Result VerifyVerificationChange(bool commandIsApproved)
    {
        if (VerificationChange is null) return Result.Fail("There is no change verification request");

        if (commandIsApproved is false)
        {
            VerificationChange.Reject();

            return Result.Success();
        }

        var verificationInfos = VerificationChange.ChangeVerificationRequestDetails
            .Select(id => Verification.Create(id.ImageUrl, Id))
            .ToList();

        _verifications = verificationInfos;

        VerificationChange.Approve();

        return Result.Success();
    }

    public void SetProfileAsVerified() => TutorStatus = TutorStatus.Active;

    public void SetTutorStatus(TutorStatus tutorStatus) => TutorStatus = tutorStatus;

    public Result Update(string university, AcademicLevel academicLevel, IList<SubjectId> updateMajors)
    {
        var result = DomainValidation.Sequentially(
            () => SetUniversity(university),
            () => SetMajors(updateMajors)
        );

        if (result.IsFailed) return result.Error;

        AcademicLevel = academicLevel;
        TutorStatus = TutorStatus.Unproven;

        DomainEvents.Add(new TutorUpdatedDomainEvent(this));

        return Result.Success();
    }

    public Result SetRate(decimal rate)
    {
        if (rate is < MinRate or > MaxRate)
            return Result.Fail($"Rate must be between {MinRate} and {MaxRate}");

        Rate = rate;

        return Result.Success();
    }

    private Result SetUniversity(string university)
    {
        if (university.Length is < MinUniversityLength or > MaxUniversityLength)
            return Result.Fail(
                $"University name must be between {MinUniversityLength} and {MaxUniversityLength} characters");

        University = university;

        return Result.Success();
    }

    private Result SetMajors(IList<SubjectId> updateMajors)
    {
        if (updateMajors.Count is < MinMajorsCount or > MaxMajorsCount)
            return Result.Fail($"Majors count must be between {MinMajorsCount} and {MaxMajorsCount}");

        // Get the new majors that are not in the tutor majors
        var newMajors = updateMajors.Where(m => _majors.All(tm => tm != m));

        // Add the new majors to the tutor majors
        _majors.AddRange(newMajors);

        // Remove tutor majors that are not in the new list
        _majors.RemoveAll(tm => updateMajors.All(m => m != tm));

        return Result.Success();
    }
}

public record TutorProfileCreatedDomainEvent(Tutor Tutor) : IDomainEvent;

public record TutorUpdatedDomainEvent(Tutor Tutor) : IDomainEvent;

public record VerificationChangeCreatedDomainEvent(Tutor Tutor) : IDomainEvent;