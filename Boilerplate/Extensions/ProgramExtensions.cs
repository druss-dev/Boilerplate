using Azure.Identity;
using Boilerplate.Options;
using Boilerplate.Repositories.Client;
using Boilerplate.Repositories.Thingabase;
using Boilerplate.Services.Thing;
using Boilerplate.Utilities;
using Caerus.Client;
using Caerus.Client.Internal;
using Caerus.Client.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Boilerplate.Extensions;

public static class ProgramExtensions
{
    /// <summary>
    /// adds configuration options to the host container
    /// </summary>
    public static void AddConfigurationOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<ApplicationOptions>()
            .Bind(builder.Configuration.GetSection(ApplicationOptions.Application))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddOptions<AzureOptions>()
            .Bind(builder.Configuration.GetSection(AzureOptions.Azure))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddOptions<SystemOptions>()
            .Bind(builder.Configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();
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
    public static void AddInternalServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IThingService, ThingService>();
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

    /// <summary>
    /// configures the caerus client
    /// </summary>
    public static void AddCaerusClient(this WebApplicationBuilder builder)
    {
        var applicationOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;
        var caerusClientConfig = new CaerusClientConfig
        {
            UserName = applicationOptions.CaerusClientUsername!,
            Password = applicationOptions.CaerusClientPassword!,
            CaerusBaseUri = applicationOptions.CaerusApiBaseUrl!
        };
        builder.Services.AddCaerusClient(caerusClientConfig);
        

        builder.Services.AddCaerusInternalClient(
            applicationOptions.InternalApiBaseUrl,
            applicationOptions.InternalServiceUserName,
            applicationOptions.InternalServicePassword);

    }

    /// <summary>
    /// adds logging configuration to the host container
    /// </summary>
    public static void AddLogging(this WebApplicationBuilder builder)
    {

        builder.Host
            .UseSerilog((_, _, loggerConfiguration) =>
            {
                // console logger
                loggerConfiguration
                    .WriteTo
                    .Console(theme: AnsiConsoleTheme.Code);
            });
    }

    /// <summary>
    /// registers an encryption provider to allow access to encrypted data source
    /// </summary>
    public static async Task AddEncryptionProvider(this WebApplication host)
    {
        await Task.Run(() =>
        {
            var azureOptions = host.Services.GetService<IOptions<AzureOptions>>()?.Value ?? new AzureOptions();

            var clientCredential = new ClientSecretCredential(azureOptions.CmkTenantId,
                azureOptions.CmkClientId, azureOptions.CmkClientSecret);

            var azureProvider = new SqlColumnEncryptionAzureKeyVaultProvider(clientCredential);

            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(
                new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>(1, StringComparer.OrdinalIgnoreCase)
                {
                    { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureProvider }
                });
        });
    }
}