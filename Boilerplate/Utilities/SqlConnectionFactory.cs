using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Boilerplate.Options;
using Boilerplate.Repositories.Client;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Boilerplate.Utilities;
public interface ISqlConnectionFactory
{
    Task<SqlConnection> CreateAsync(string clientId);
}
public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IClientRepository _clientRepository;
    private readonly SystemOptions _systemOptions;
    private readonly ConcurrentDictionary<string, string> _connectionStrings = new ConcurrentDictionary<string, string>();
    private readonly ILogger<SqlConnectionFactory> _logger;

    public SqlConnectionFactory(
        IClientRepository clientRepository, ILogger<SqlConnectionFactory> logger, IOptions<SystemOptions> systemOptions)
    {
        _clientRepository = clientRepository;
        _logger = logger;
        _systemOptions = systemOptions.Value;
    }

    public async Task<SqlConnection> CreateAsync(string clientId)
    {
        if (!_connectionStrings.TryGetValue(clientId, out var connString))
        {
            var client = await _clientRepository.GetClientAsync(clientId, _systemOptions.SqlMasterConnectionString!);
            if (string.IsNullOrWhiteSpace(client?.DatabaseName))
            {
                _logger.LogError("No database name found for client: {ClientId}", clientId);
                throw new Exception($"No database name found for client: {clientId}");
            }
                
            var clientNumber = Regex.Match(client.DatabaseName, @"\d+").Value;
            connString = _systemOptions.SqlConnectionString?.Replace("{0}", clientNumber);
            
            if (string.IsNullOrEmpty(connString))
            {
                _logger.LogError("Failed to get the connection string to the database");
                throw new Exception($"Failed to get the connection string to the database");
            }

            // adjust if timeout is being exceeded
            // if (!connString.Contains("Command Timeout"))
            // {
            //     connString += ";Command Timeout=600";
            // }
            
            _connectionStrings.TryAdd(clientId, connString);

        }
        return new SqlConnection(connString);
    }
}