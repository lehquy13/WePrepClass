using Matt.ResultObject;
using Matt.SharedKernel;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;
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
    private readonly List<Major> _majors = [];

    public UserId UserId { get; private set; } = null!;

    public AcademicLevel AcademicLevel { get; private set; } = AcademicLevel.UnderGraduate;
    public TutorStatus TutorStatus { get; private set; }
    public string University { get; private set; } = string.Empty;
    public bool IsVerified { get; private set; }
    public decimal? Rate { get; private set; }

    public VerificationChange? VerificationChange { get; private set; }

    public IReadOnlyList<Verification> Verifications => _verifications.AsReadOnly();
    public IReadOnlyList<Major> Majors => _majors.AsReadOnly();

    private Tutor()
    {
    }

    public static Result<Tutor> Create(
        UserId userId,
        AcademicLevel academicLevel,
        string university,
        IList<Major> majors,
        bool isVerified = false)
    {
        var id = TutorId.Create();

        var tutor = new Tutor
        {
            Id = id,
            UserId = userId,
            AcademicLevel = academicLevel,
            IsVerified = isVerified,
            TutorStatus = TutorStatus.Active
        };

        var result = tutor.ValidateSequentially(
            () => tutor.SetUniversity(university),
            () => tutor.SetMajors(majors)
        );

        if (result.IsFailure) return result.Error;

        tutor.DomainEvents.Add(new TutorProfileCreatedDomainEvent(tutor));

        return tutor;
    }

    public Result ChangeVerification(List<string> urls)
    {
        var verificationChange = VerificationChange.Create(Id, urls);

        if (verificationChange.IsFailure) return verificationChange.Error;

        if (_verifications.Count == 0)
        {
            _verifications = urls.Select(url => Verification.Create(url, Id)).ToList();
            IsVerified = false;
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

    public void SetProfileAsVerified() => IsVerified = true;

    public void SetTutorStatus(TutorStatus tutorStatus) => TutorStatus = tutorStatus;

    public Result Update(string university, AcademicLevel academicLevel, IList<Major> updateMajors)
    {
        var result = this.ValidateSequentially(
            () => SetUniversity(university),
            () => SetMajors(updateMajors)
        );

        if (result.IsFailure) return result.Error;

        AcademicLevel = academicLevel;
        IsVerified = false;

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

    private Result SetMajors(IList<Major> updateMajors)
    {
        if (updateMajors.Count is < MinMajorsCount or > MaxMajorsCount)
            return Result.Fail($"Majors count must be between {MinMajorsCount} and {MaxMajorsCount}");

        // Get the new majors that are not in the tutor majors
        var newMajors = updateMajors.Where(m => _majors.All(tm => tm.SubjectId != m.SubjectId));

        // Add the new majors to the tutor majors
        _majors.AddRange(newMajors);

        // Remove tutor majors that are not in the new list
        _majors.RemoveAll(tm => updateMajors.All(m => m.SubjectId != tm.SubjectId));

        return Result.Success();
    }
}

public record TutorProfileCreatedDomainEvent(Tutor Tutor) : IDomainEvent;

public record TutorUpdatedDomainEvent(Tutor Tutor) : IDomainEvent;

public record VerificationChangeCreatedDomainEvent(Tutor Tutor) : IDomainEvent;