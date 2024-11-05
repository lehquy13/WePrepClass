using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class TutorSpecification : ValueObject
{
    public GenderOption TutorGender { get; init; } = GenderOption.None;
    public AcademicLevelOption TutorAcademicLevel { get; init; } = AcademicLevelOption.Optional;

    private TutorSpecification()
    {
    }

    public static TutorSpecification Create(GenderOption tutorGender, AcademicLevelOption tutorAcademicLevel)
    {
        return new TutorSpecification
        {
            TutorGender = tutorGender,
            TutorAcademicLevel = tutorAcademicLevel
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TutorGender;
        yield return TutorAcademicLevel;
    }
}