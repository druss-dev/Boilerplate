using ApolloFramework;

namespace Boilerplate.Services.Apollo;

public class ApolloService : IApolloService
{
    public async Task TestApollo()
    {
        var context = new ApolloContext();
        var connection = Environment.GetEnvironmentVariable("DATAVIEW_CONNECTION_STRING");

        try
        {
            var results = await context
                .FromDataView(connection!)
                .Patients
                .Take(10)
                .ToArrayAsync();

            var test = 1;
        }
        catch(Exception ex)
        {
            throw new Exception("An apollo exception has occurred", ex);
        }
    }
}