using Mapster;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Contracts.Users;

public class LearningCourseForDetailDtoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(Course, Subject, Guid, User), AttendedCourseDetailDto>()
            .Map(des => des.Id, src => src.Item1.Id.Value)
            .Map(des => des.TutorId, src => src.Item3)
            .Map(des => des.TutorName, src => src.Item4.GetFullName())
            .Map(des => des.TutorContact, src => src.Item4.Description)
            .Map(des => des.TutorEmail, src => src.Item4.Email)
            .Map(des => des.Title, src => src.Item1.Title)
            .Map(des => des.Status, src => src.Item1.Status.ToString())
            .Map(des => des.LearningMode, src => src.Item1.LearningModeRequirement.ToString())
            .Map(des => des.ChargeFee, src => src.Item1.ChargeFee.Amount)
            .Map(des => des.SectionFee, src => src.Item1.SessionFee.Amount)
            .Map(des => des.SessionDurationDisplay, src => src.Item1.Session.DisplayValue)
            .Map(des => des.SubjectName, src => src.Item2.Name)
            .Map(des => des.Address, src => src.Item1.Address)
            .Map(des => des.Description, src => src.Item1.Description)
            .Map(des => des.Detail, src => src.Item1.Review == null ? "" : src.Item1.Review.Detail)
            .Map(des => des.Rate, src => src.Item1.Review == null ? (short)5 : src.Item1.Review.Rate)
            .Map(des => des, src => src);
    }
}