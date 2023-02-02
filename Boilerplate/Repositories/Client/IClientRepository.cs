using Boilerplate.Models;

namespace Boilerplate.Repositories.Client;

public interface IClientRepository
{
    Task<ClientDatabase?> GetClientAsync(string clientId, string sqlConnectionString);
}