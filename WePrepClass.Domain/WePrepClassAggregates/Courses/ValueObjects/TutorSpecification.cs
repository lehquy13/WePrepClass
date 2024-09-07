using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class TutorSpecification : ValueObject
{
    private GenderOption TutorGender { get; init; } = GenderOption.None;
    private AcademicLevel TutorAcademicLevel { get; init; } = AcademicLevel.Optional;

    private TutorSpecification()
    {
    }

    public static TutorSpecification Create(GenderOption tutorGender, AcademicLevel tutorAcademicLevel)
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