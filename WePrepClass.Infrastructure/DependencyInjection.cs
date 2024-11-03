using System.Text;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Domain.Interfaces.Emails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using WePrepClass.Application.Interfaces;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Infrastructure.AppLogger;
using WePrepClass.Infrastructure.Authentication;
using WePrepClass.Infrastructure.Cloudinary;
using WePrepClass.Infrastructure.EmailServices;
using WePrepClass.Infrastructure.Middleware;
using WePrepClass.Infrastructure.Models;
using WePrepClass.Infrastructure.Persistence.EntityFrameworkCore;
using WePrepClass.Infrastructure.Persistence.Repositories;
using IdentityDbContext = Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext;

namespace WePrepClass.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Authentication configuration using jwt bearer 
        services.AddAuthentication(configuration);
        IdentityModelEventSource.ShowPII = true;

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(480);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // set configuration settings to emailSettingName and turn it into Singleton
        var emailSettingNames = new EmailSettingNames();
        configuration.Bind(EmailSettingNames.SectionName, emailSettingNames);
        services.AddSingleton(Options.Create(emailSettingNames));

        // set configuration settings to cloudinarySettings and turn it into Singleton
        var cloudinary = new CloudinarySetting();
        configuration.Bind(CloudinarySetting.SectionName, cloudinary);
        services.AddSingleton(Options.Create(cloudinary));
        services.AddScoped<IBlobStorageServices, BlobStorageServices>();

        services.AddScoped<IEmailSender, EmailSender>();

        //configure BackgroundService
        //services.AddHostedService<InfrastructureBackgroundService>();
        return services;
    }

    public static IServiceCollection AddPersistence(
        this IServiceCollection services, ConfigurationManager configuration
    )
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultIdentityConnection"))
        );

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddScoped<IReadDbContext, ReadDbContext>();

        services
            .AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
        //services.AddScoped<TenantDatabaseEnsureCreatedMiddleware>();

        return services;
    }

    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // set configuration settings to jwtSettings and turn it into Singleton
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IValidator, Validator>();

        services.AddAuthentication(scheme =>
            {
                scheme.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                scheme.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                scheme.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        try
                        {
                            var token = context.HttpContext.Request.Headers[HeaderNames.Authorization]
                                .FirstOrDefault()?.Split(" ").Last();
                            if (token != null)
                            {
                                context.Token = token;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdministratorRole", policy => { policy.RequireRole("Admin"); })
            .AddPolicy("RequireTutorRole", policy => { policy.RequireRole("Tutor"); });
    }
}

public static class RequestPipeline
{
    public static void AddInfrastructureMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
    }
}