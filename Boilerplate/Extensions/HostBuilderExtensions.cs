using Azure.Identity;
using Boilerplate.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Boilerplate.Extensions;

public static class HostBuilderExtensions
{    
    /// <summary>
    /// handles creation of the default host builder
    /// </summary>
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        IHostBuilder builder;
        
        builder = Host
            .CreateDefaultBuilder(args)
            .ConfigureHostBuilder()
            .AddLogging();


        return builder;
    }
    
    /// <summary>
    /// handles application scoped configuration and dependency injection
    /// </summary>
    public static IHostBuilder ConfigureHostBuilder(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddConfigurationOptions(context.Configuration)
                .AddExternalServices(context.Configuration)
                .AddInternalServices()
                .AddDataRepositories();
        });

        return builder;
    }
    
    /// <summary>
    /// handles configuration of application logging
    /// </summary>
    public static IHostBuilder AddLogging(this IHostBuilder builder)
    {
        builder
            .UseSerilog((_, _, loggerConfiguration) =>
            {
                // console logger
                loggerConfiguration
                    .WriteTo
                    .Console(theme: AnsiConsoleTheme.Code);
            });

        return builder;
    }
    
    public static async Task AddEncryptionProvider(this IHost host)
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