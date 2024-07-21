using Mapster;
using MapsterMapper;
using WePrepClass.Application;
using WePrepClass.Contracts;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;

namespace WePrepClass.UnitTestSetup;

public static class SubjectTestData
{
    public static IEnumerable<Subject> Subjects =>
    [
        Subject.Create("Math", "Mathematics").Value,
        Subject.Create("Science", "Science").Value,
        Subject.Create("English", "English").Value
    ];
}

public static class MapsterProfile
{
    public static IMapper Get => ScanMapsterProfile();

    private static Mapper ScanMapsterProfile()
    {
        var config = TypeAdapterConfig.GlobalSettings;
            
        config.Scan(typeof(DependencyInjection).Assembly);
        config.Scan(typeof(BasicAuditedEntityDto<>).Assembly);

        return new Mapper(config);
    }
}