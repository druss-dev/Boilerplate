using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Models;
using Boilerplate.Options;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Boilerplate.Repositories.Client;

public class ClientRepository : IClientRepository
{
    private readonly SystemOptions _systemOptions;
    private readonly ConcurrentDictionary<string, ClientDatabase> _clients = new ConcurrentDictionary<string, ClientDatabase>();

    public ClientRepository(IOptions<SystemOptions> systemOptions)
    {
        _systemOptions = systemOptions.Value;
    }

    public async Task<ClientDatabase?> GetClientAsync(string clientId, string sqlConnectionString)
    {
        //TODO make this a stored procedure 
        const string sql = @"select * from [dbo].[Clients] where cuid = @clientId";
        if (!_clients.TryGetValue(clientId, out var client))
        {
            using (var conn = new SqlConnection(sqlConnectionString))
                client = await conn.QueryFirstOrDefaultAsync<ClientDatabase>(sql, new { clientId });
            if (client != null)
                _clients.TryAdd(clientId, client);
        }
        return client;
    }
}