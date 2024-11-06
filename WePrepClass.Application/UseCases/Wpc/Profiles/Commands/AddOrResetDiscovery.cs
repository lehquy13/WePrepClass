using Matt.SharedKernel.Application.Mediators.Commands;

namespace WePrepClass.Application.UseCases.Wpc.Profiles.Commands;

public record AddOrResetDiscoveryCommand(Guid UserId, List<Guid> DiscoveryIds) : ICommandRequest;
//
// public class AddOrResetDiscoveryCommandHandler(
//     IUnitOfWork unitOfWork,
//     ICurrentUserService currentUserService,
//     IRepository<Discovery, DiscoveryId> discoveryRepository,
//     IAppLogger<AddOrResetDiscoveryCommandHandler> logger,
//     IAsyncQueryableExecutor asyncQueryableExecutor,
//     IRepository<DiscoveryUser, DiscoveryUserId> discoveryUserRepository)
//     : CommandHandlerBase<AddOrResetDiscoveryCommand>(unitOfWork, logger)
// {
//     public override async Task<Result> Handle(AddOrResetDiscoveryCommand request, CancellationToken cancellationToken)
//     {
//         var userId = CustomerId.Create(currentUserService.UserId);
//
//         var discoveryQuery = discoveryUserRepository.GetAll()
//             .Join(discoveryRepository.GetAll(),
//                 discoveryUser => discoveryUser.DiscoveryId,
//                 discovery => discovery.Id,
//                 (discoveryUser, discovery) => new { discoveryUser, discovery });
//
//         var userDiscovery =
//             await asyncQueryableExecutor.ToListAsync(discoveryQuery, true, cancellationToken);
//
//         var discoverIds = userDiscovery.Select(x => x.discovery.Id.Value);
//
//         // select the discoveries that are not in the user's discovery list
//         var discoveriesNotInUserDiscoveryList = request.DiscoveryIds
//             .Where(discoveryId => !discoverIds.Contains(discoveryId))
//             .Select(discoveryId => DiscoveryUser.Create(DiscoveryId.Create(discoveryId), userId))
//             .ToList();
//
//         await discoveryUserRepository.InsertManyAsync(discoveriesNotInUserDiscoveryList, cancellationToken);
//
//         if (await UnitOfWork.SaveChangesAsync(cancellationToken) <= 0)
//         {
//             return Result.Fail(UserProfileAppServiceError.FailToAddOrResetDiscoveryErrorWhileSavingChanges);
//         }
//
//         return Result.Success();
//     }
// }