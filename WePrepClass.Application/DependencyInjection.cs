using System.Reflection;
using WePrepClass.Domain;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Matt.SharedKernel.Application.Authorizations;
using Matt.SharedKernel.Application.Validations;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.DependencyInjection;

namespace WePrepClass.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;

        services
            .AddMediator(applicationAssembly)
            .AddValidatorsFromAssembly(applicationAssembly) // Handle base validation of application layer
            //.AddValidatorsFromAssembly(applicationAssembly) // Handle validation of specific application layer
            .AddApplicationMappings(applicationAssembly)
            .AddLazyCache();

        return services;
    }

    private static IServiceCollection AddMediator(this IServiceCollection services, Assembly applicationAssembly)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                applicationAssembly,
                typeof(DependencyInjection).Assembly,
                typeof(DomainDependencyInjection).Assembly);

            cfg.NotificationPublisher = new TaskWhenAllPublisher();

            cfg.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        });

        return services;
    }

    private static IServiceCollection AddApplicationMappings(this IServiceCollection services,
        Assembly applicationAssembly)
    {
        var config = TypeAdapterConfig.GlobalSettings;
            
        config.Scan(applicationAssembly, typeof(DependencyInjection).Assembly);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }

    public static IEnumerable<Assembly> GetApplicationCoreAssemblies =>
    [
        typeof(DependencyInjection).Assembly,
        typeof(DomainDependencyInjection).Assembly
    ];
}