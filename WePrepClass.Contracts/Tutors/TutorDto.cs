using Mapster;
using Matt.SharedKernel.Application.Contracts.Primitives;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Contracts.Tutors;

public class TutorDto : EntityDto<Guid>
{
    public string FullName { get; init; } = null!;
    public string Gender { get; init; } = null!;
    public int BirthYear { get; init; }
    public string Address { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string AcademicLevel { get; init; } = null!;
    public string University { get; init; } = null!;
    public decimal? Rate { get; init; }
    public List<string> TutorMajors { get; init; } = [];
    public List<ReviewDto> Reviews { get; set; } = null!;
}

public class ReviewDto : BasicAuditedEntityDto<Guid>
{
    public string Participant { get; init; } = null!;
    public string Subject { get; init; } = null!;
    public short Rate { get; init; }
    public string Detail { get; init; } = null!;
}

public class TutorDetailForClientDtoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(Tutor, User), TutorDto>()
            .Map(dest => dest.Id, src => src.Item1.Id.Value)
            .Map(dest => dest.FullName, src => src.Item2.GetFullName())
            .Map(dest => dest.AcademicLevel, src => src.Item1.AcademicLevel.ToString())
            .Map(dest => dest.Rate, src => src.Item1.Rate)
            .Map(dest => dest.University, src => src.Item1.University)
            .Map(dest => dest.Address, src => src.Item2.Address.City + src.Item2.Address)
            //.Map(dest => dest.TutorMajors, src => src.Item1.Majors.Select(x => x.SubjectName))
            .Map(dest => dest, src => src.Item2)
            .Map(dest => dest, src => src);
    }
}