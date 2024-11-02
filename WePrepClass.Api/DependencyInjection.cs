using System.Reflection;
using Matt.AutoDI;
using WePrepClass.Api.Middlewares;

namespace WePrepClass.Api;

public static class DependencyInjection
{
    public static void AddPresentation(this IServiceCollection services)
        => services
            .AddCors()
            .RegisterExceptionHandler()
            .RegisterAutoDi()
            .AddProblemDetails();

    private static IServiceCollection RegisterExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();

        return services;
    }

    private static IServiceCollection RegisterAutoDi(this IServiceCollection services)
    {
        List<Assembly> assemblies =
        [
            typeof(Infrastructure.DependencyInjection).Assembly
        ];

        assemblies.AddRange(Application.DependencyInjection.GetApplicationCoreAssemblies);

        services.AddServiced(assemblies.ToArray());

        return services;
    }
}