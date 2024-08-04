using Mapster;
using MapsterMapper;
using WePrepClass.Application;
using WePrepClass.Contracts;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.UnitTestSetup;

public static class TestData
{
    public static class UserData
    {
        public const string UserName = "JohnDoe";
        public const string FirstName = "John";
        public const string LastName = "Doe";
        public const string Password = "1q2w3E**";
        public const Gender UserGender = Gender.Female;
        public const Role UserRole = Role.BaseUser;
        public const string Mail = "johnd@mail.com";
        public const string PhoneNumber = "0123456789";
        public const int BirthYear = 1991;
        public const string City = "Hanoi";
        public const string District = "Ba Dinh";
        public const string Street = "123 Hoang Hoa Tham Street";
        public static readonly UserId UserId = UserId.Create();

        public static readonly User ValidUser =
            User.Create(
                UserId,
                FirstName,
                LastName,
                UserGender,
                BirthYear,
                Address.Create(City, District, Street).Value,
                string.Empty,
                null,
                Mail,
                PhoneNumber,
                UserRole).Value;
    }

    public static class SubjectData
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