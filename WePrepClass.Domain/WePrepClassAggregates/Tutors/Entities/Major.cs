// using Matt.SharedKernel.Domain.Primitives;
// using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
// using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
//
// namespace WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;
//
// public class Major : Entity<MajorId>
// {
//     public TutorId TutorId { get; private set; } = null!;
//     public SubjectId SubjectId { get; private set; } = null!;
//
//     private Major()
//     {
//     }
//
//     public static Major Create(TutorId tutorId, SubjectId subjectId)
//     {
//         return new Major
//         {
//             Id = MajorId.Create(),
//             TutorId = tutorId,
//             SubjectId = subjectId
//         };
//     }
// }