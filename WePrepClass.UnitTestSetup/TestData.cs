using Mapster;
using MapsterMapper;
using WePrepClass.Application;
using WePrepClass.Contracts;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;
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
        private const Role UserRole = Role.BaseUser;
        public const string Mail = "johnd@mail.com";
        private const string PhoneNumber = "0123456789";
        public const int BirthYear = 1991;
        private const string City = "Hanoi";
        private const string District = "Ba Dinh";
        private const string Street = "123 Hoang Hoa Tham Street";
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

    public static class MajorData
    {
        public static Major Major1 => Major.Create(
            TutorData.Tutor1.Id,
            SubjectData.Subjects[0].Id);
    }

    public static class TutorData
    {
        public static Tutor Tutor1
        {
            get
            {
                
                var tutor = Tutor.Create(
                        UserData.UserId,
                        AcademicLevel.UnderGraduate,
                        "University of Science",
                        new List<SubjectId> { SubjectData.Subjects[0].Id })
                    .Value;
                
                tutor.SetTutorStatus(TutorStatus.Active);
                
                return tutor;
            }
        }
    }

    public static class CourseData
    {
        public static Course Course1 => Course.Create(
                "Course Title With More Than 50 Characters Length To Test The Validation",
                "Course 1 Description",
                LearningMode.Hybrid,
                Fee.Create(10m, CurrencyCode.USD),
                Fee.Create(10m, CurrencyCode.USD),
                LearnerDetail.Create(
                    "John Doe",
                    Gender.Female,
                    "0123123123",
                    2
                ),
                TutorSpecification.Create(
                    GenderOption.Female,
                    AcademicLevel.Graduated),
                Session.Create(90m).Value,
                Address.Create("City", "Country", "Detail").Value,
                SubjectData.Subjects[0].Id
            )
            .Value;
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