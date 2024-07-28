using Matt.ResultObject;
using Matt.SharedKernel;
using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Subjects;

public class Subject : FullAuditedAggregateRoot<SubjectId>
{
    private const int MaxNameLength = 30;
    private const int MinNameLength = 3;

    private const int MaxDescriptionLength = 100;
    private const int MinDescriptionLength = 10;

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private Subject()
    {
    }

    public static Result<Subject> Create(string name, string description)
    {
        var subject = new Subject();

        var result = DomainValidation.Sequentially(
            () => subject.SetName(name),
            () => subject.SetDescription(description)
        );

        if (result.IsFailure) return result.Error;

        return subject;
    }

    public Result SetName(string name)
    {
        if (name.Length is < MinNameLength or > MaxNameLength)
            return Result.Fail($"Name length must be between {MinNameLength} and {MaxNameLength} characters");

        Name = name;

        return Result.Success();
    }

    public Result SetDescription(string description)
    {
        if (description.Length is < MinDescriptionLength or > MaxDescriptionLength)
            return Result.Fail(
                $"Description length must be between {MinDescriptionLength} and {MaxDescriptionLength} characters");

        Description = description;

        return Result.Success();
    }

    public void SetAsDeleted()
    {
        IsDeleted = true;
    }
}