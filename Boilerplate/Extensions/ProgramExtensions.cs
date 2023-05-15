using ApolloFramework.Extensions;
using Azure.Identity;
using Boilerplate.Options;
using Boilerplate.Repositories.Client;
using Boilerplate.Repositories.Thingabase;
using Boilerplate.Services.Apollo;
using Boilerplate.Services.Thing;
using Boilerplate.Utilities;
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
    
    /// <summary>
    /// configures all application dependent services
    /// </summary>
    public static void AddDependentServices(this WebApplicationBuilder builder)
    {
        builder.AddInternalServices();
        builder.AddExternalServices();
    }
    
    /// <summary>
    /// configures all externally sourced services to the dependency injection container
    /// </summary>
    public static void AddExternalServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddApollo();
    }
    
    /// <summary>
    /// configures all internally sourced services to the dependency injection container
    /// </summary>
    public static void AddInternalServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IThingService, ThingService>();
        builder.Services.AddTransient<IApolloService, ApolloService>();
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