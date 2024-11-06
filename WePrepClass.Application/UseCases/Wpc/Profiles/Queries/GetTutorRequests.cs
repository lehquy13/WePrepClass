using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Profiles.Queries;

public record GetTutorRequestsQuery : IQueryRequest<IEnumerable<TutorRequestForListDto>>, IAuthorizationRequired;

public class GetTutorRequestsQueryHandler(
    IReadDbContext dbContext,
    IAppLogger<GetTutorRequestsQueryHandler> logger,
    ICurrentUserService currentUserService,
    IMapper mapper
) : QueryHandlerBase<GetTutorRequestsQuery, IEnumerable<TutorRequestForListDto>>(logger, mapper)
{
    public override async Task<Result<IEnumerable<TutorRequestForListDto>>> Handle(GetTutorRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(currentUserService.UserId);

        var queryable =
            from req in dbContext.TutoringRequests
            where req.UserId == userId
            join tutor in dbContext.Tutors on req.TutorId equals tutor.Id
            join user in dbContext.Users on tutor.UserId equals user.Id
            orderby req.TutorRequestStatus descending
            select new TutorRequestForListDto
            {
                Id = req.Id.Value,
                RequestMessage = req.Message,
                Status = req.TutorRequestStatus.ToString(),
                TutorFullName = user.FirstName + " " + user.LastName,
                CreationTime = req.CreationTime
            };

        return await queryable.ToListAsync(cancellationToken);
    }
}

public class TutorRequestForListDto
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; }
    public string TutorFullName { get; set; } = string.Empty;
    public string RequestMessage { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}