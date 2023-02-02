using Boilerplate.Options;
using Boilerplate.Repositories.Client;
using Boilerplate.Repositories.Thingabase;
using Boilerplate.Services.Thing;
using Boilerplate.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///  creates options for each section of environment variables
    /// </summary>
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ApplicationOptions>()
            .Bind(configuration.GetSection(ApplicationOptions.Application))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<AzureOptions>()
            .Bind(configuration.GetSection(AzureOptions.Azure))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SystemOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
    
    /*
    /// <summary>
    /// configures all externally sourced services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
    */
    
    /// <summary>
    /// configures all internally sourced services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddInternalServices(this IServiceCollection services)
    {
        services.AddTransient<IThingService, ThingService>();

        return services;
    }
    
    /// <summary>
    /// configures all repositories to the dependency injection container
    /// </summary>
    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IClientRepository, ClientRepository>();
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddTransient<IThingabaseRepository, ThingabaseRepository>();
       
        return services;
    }
}