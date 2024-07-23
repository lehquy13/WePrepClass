using Mapster;
using MapsterMapper;
using WePrepClass.Application;
using WePrepClass.Contracts;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;

namespace WePrepClass.UnitTestSetup;

public static class SubjectTestData
{
    public static List<Subject> Subjects =>
    [
        Subject.Create("Math", "Mathematics").Value,
        Subject.Create("Science", "Science Subject").Value,
        Subject.Create("English", "English Subject").Value,
        Subject.Create("Physics", "Physics Subject").Value,
        Subject.Create("Chemistry", "Chemistry Subject").Value,
        Subject.Create("Biology", "Biology Subject").Value,
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