// TODO: pending
// using MapsterMapper;
// 
// using Matt.SharedKernel.Application.Contracts.Interfaces;
// using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
// using Matt.SharedKernel.Application.Mediators.Queries;
// using Matt.SharedKernel.Domain.Interfaces;
// using WePrepClass.Application.Interfaces;
// using WePrepClass.Domain.Commons.Enums;
// using WePrepClass.Domain.WePrepClassAggregates.TeachingRequests.ValueObjects;
//
// namespace WePrepClass.Application.UseCases.Wpc.TutorProfiles.Queries;
//
// public record GetCourseRequestDetailQuery(Guid TeachingRequestId)
//     : IQueryRequest<CourseRequestForDetailDto>, IAuthorizationRequired;
//
// public class GetCourseRequestDetailQueryHandler(
//     IReadDbContext dbContext,
//     ICurrentUserService currentUserService,
//     ILogger<GetCourseRequestDetailQueryHandler> logger,
//     IMapper mapper)
//     : QueryHandlerBase<GetCourseRequestDetailQuery, CourseRequestForDetailDto>
// {
//     public override async Task<Result<CourseRequestForDetailDto>> Handle(GetCourseRequestDetailQuery request,
//         CancellationToken cancellationToken)
//     {
//         var teachingRequestQueryable =
//             from teachingRequest in dbContext.TeachingRequests
//             join course in dbContext.Courses on teachingRequest.CourseId equals course.Id
//             join subject in dbContext.Subjects on course.SubjectId equals subject.Id
//             where teachingRequest.Id == TeachingRequestId.Create(request.TeachingRequestId
//             select new CourseRequestForDetailDto()
//             {
//                 TutorId = tutor.Id.Value,
//                 CourseId = teachingRequestQueryable.Id.Value,
//                 Title = teachingRequestQueryable.Title,
//                 SubjectName = subject.Name,
//                 Description = teachingRequestQueryable.Description
//             }
//
//         var teachingRequestResult = await teachingRequestQueryable.FirstOrDefaultAsync(cancellationToken);
//
//         var courseRequestDto = ;
//
//         if (teachingRequestQueryable.Status == Status.Confirmed && teachingRequestQueryable.TutorId == tutor.Id)
//         {
//             courseRequestDto.LearnerName = teachingRequestQueryable.LearnerName;
//             courseRequestDto.LearnerContact = teachingRequestQueryable.ContactNumber;
//             courseRequestDto.RequestStatus = RequestStatus.Done.ToString();
//
//             return courseRequestDto;
//         }
//
//         if (courseRequest is not null)
//         {
//             courseRequestDto.Id = courseRequest.Id.Value;
//
//             if (teachingRequestQueryable.TutorId == tutor.Id)
//             {
//                 if (teachingRequestQueryable.Status == Status.Confirmed)
//                 {
//                     courseRequestDto.LearnerName = teachingRequestQueryable.LearnerName;
//                     courseRequestDto.LearnerContact = teachingRequestQueryable.ContactNumber;
//                     courseRequestDto.RequestStatus = RequestStatus.Done.ToString();
//                 }
//                 else
//                 {
//                     courseRequestDto.RequestStatus = RequestStatus.OnProgress.ToString();
//                 }
//
//                 return courseRequestDto;
//             }
//
//             courseRequestDto.RequestStatus = teachingRequestQueryable.TutorId == null
//                 ? RequestStatus.Pending.ToString()
//                 : RequestStatus.Canceled.ToString();
//
//             return courseRequestDto;
//         }
//
//         if (teachingRequestQueryable.TutorId != tutor.Id)
//         {
//             return Result.Fail("You have no permission to view related course request.");
//         }
//
//         if (teachingRequestQueryable.Status == Status.Confirmed)
//         {
//             courseRequestDto.LearnerName = teachingRequestQueryable.LearnerName;
//             courseRequestDto.LearnerContact = teachingRequestQueryable.ContactNumber;
//             courseRequestDto.RequestStatus = RequestStatus.Done.ToString();
//         }
//         else
//         {
//             courseRequestDto.RequestStatus = RequestStatus.OnProgress.ToString();
//         }
//
//         return courseRequestDto;
//     }
// }

