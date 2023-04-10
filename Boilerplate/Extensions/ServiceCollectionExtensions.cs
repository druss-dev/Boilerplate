using Boilerplate.Options;
using Boilerplate.Repositories.Client;
using Boilerplate.Repositories.Patient;
using Boilerplate.Repositories.Thingabase;
using Boilerplate.Services.Patient;
using Boilerplate.Services.Thing;
using Boilerplate.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatientApiService;
using PatientApiService.Options;

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
        
        services.AddOptions<PatientOptions>()
            .Bind(configuration.GetSection(PatientOptions.Patient))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SystemOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
    
    
    /// <summary>
    /// configures all externally sourced services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(nameof(PatientApiClient))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
        services.AddSingleton<IPatientApiClient, PatientApiClient>(provider =>
        {
            var patientOptions = provider.GetService<IOptions<PatientOptions>>();
            var logger = provider.GetService<ILogger<PatientApiClient>>()!;
            var factory = provider.GetService<IHttpClientFactory>()!;
            
            return new PatientApiClient(new OptionsWrapper<PatientApiOptions>(new PatientApiOptions
            {
                BaseUrl = patientOptions?.Value.BaseUrl!,
                Username = patientOptions?.Value.Username!,
                Password = patientOptions?.Value.Password!
            }), factory, logger);
        });
        return services;
    }
    
    
    /// <summary>
    /// configures all internally sourced services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddInternalServices(this IServiceCollection services)
    {
        services.AddTransient<IPatientService, PatientService>();
        services.AddTransient<IThingService, ThingService>();

        return services;
    }
    
    /// <summary>
    /// configures all repositories to the dependency injection container
    /// </summary>
    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IClientRepository, ClientRepository>();
        services.AddTransient<IPatientRepository, PatientRepository>();
        services.AddSingleton<IPatientSqlConnectionFactory, PatientSqlConnectionFactory>();
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddTransient<IThingabaseRepository, ThingabaseRepository>();
       
        return services;
    }
}