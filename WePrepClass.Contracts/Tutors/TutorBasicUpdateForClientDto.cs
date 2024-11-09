using Mapster;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;

namespace WePrepClass.Contracts.Tutors;

public class TutorBasicUpdateForClientDto
{
    public string AcademicLevel { get; set; } = Domain.Commons.Enums.AcademicLevel.Graduated.ToString();
    public string University { get; set; } = null!;
    public List<int> MajorIds { get; set; } = new();
}

public class TutorBasicUpdateDtoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TutorBasicUpdateForClientDto, Tutor>()
            .Map(des => des.University, src => src.University)
            .Map(des => des.AcademicLevel, src => src.AcademicLevel);
    }
}