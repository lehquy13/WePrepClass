using MapsterMapper;
using Matt.SharedKernel.Domain.Interfaces;
using MediatR;

namespace WePrepClass.Application;

public abstract class BaseAppService<T>(IMapper mapper, IUnitOfWork unitOfWork, IAppLogger<BaseAppService<T>> logger)
    where T : BaseAppService<T>
{
    protected readonly IMapper Mapper = mapper;
    protected readonly IUnitOfWork UnitOfWork = unitOfWork;
    protected readonly IAppLogger<BaseAppService<T>> Logger = logger;
}

public abstract class BaseCommandAppService<T>(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IAppLogger<BaseAppService<T>> logger,
    IPublisher publisher
) : BaseAppService<T>(mapper, unitOfWork, logger) where T : BaseCommandAppService<T>
{
    //protected readonly IAppCache _cache;
    protected readonly IPublisher Publisher = publisher;
}