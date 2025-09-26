using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using CT.Application.Behaviors;
using CT.Application.Extensions;
using CT.Application.Interfaces;
using CT.Application.Services;
using CT.Repository;
using System.Globalization;
using System.Reflection;

namespace CT.Application.Extensions;

public static class ConfigureApplicationServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services, string dbConnectionString, Action<IServiceCollection>? mediatrAddingBehaviors = null, Action<IServiceCollection>? mediatrAddedBehaviors = null)
    {
        var applicationAssembly = Assembly.GetExecutingAssembly();

        services.AddLogging();
        services.AddMediatR(applicationAssembly, mediatrAddingBehaviors, mediatrAddedBehaviors);
        services.AddFluentValidation(applicationAssembly);
        services.AddRepository(dbConnectionString);

        services.AddScoped<IGadgetsRepositoryService, GadgetsRepositoryService>();
        services.AddScoped<IIdentityServerRepositoryService, IdentityServerRepositoryService>();
        services.AddScoped<IIdentityServerService, IdentityServerService>();
        
        services.AddSingleton<ITokenGenerator, TokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }

    private static IServiceCollection AddRepository(this IServiceCollection services, string dbConnectionString)
    {
        if (string.IsNullOrWhiteSpace(dbConnectionString))
        {
            throw new ArgumentException("DbConnectionString not provided", nameof(dbConnectionString));
        }

        services.AddDbContext<GadgetsDbContext>(delegate (DbContextOptionsBuilder options)
        {
            options.UseSqlServer(dbConnectionString, delegate (SqlServerDbContextOptionsBuilder o)
            {
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            });
        });

        services.AddDbContextFactory<GadgetsDbContext>(delegate (DbContextOptionsBuilder options)
        {
            options.UseSqlServer();
        });

        services.AddDbContext<IsDbContext>(delegate (DbContextOptionsBuilder options)
        {
            options.UseSqlServer(dbConnectionString, delegate (SqlServerDbContextOptionsBuilder o)
            {
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            });
        });

        services.AddDbContextFactory<IsDbContext>(delegate (DbContextOptionsBuilder options)
        {
            options.UseSqlServer();
        });

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services, Assembly applicationAssembly, Action<IServiceCollection>? mediatrAddingBehaviors = null, Action<IServiceCollection>? mediatrAddedBehaviors = null)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssemblies(applicationAssembly));
        services.AddFluentValidation(applicationAssembly);

        mediatrAddingBehaviors?.Invoke(services);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionsBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestAuthenticationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

        mediatrAddedBehaviors?.Invoke(services);

        return services;
    }

    private static IServiceCollection AddFluentValidation(this IServiceCollection services, Assembly applicationAssembly)
    {
        services.AddValidatorsFromAssembly(applicationAssembly);

        ValidatorOptions.Global.LanguageManager.Enabled = false;
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

        return services;
    }
}
