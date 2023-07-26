using Caerus.Client;
using Caerus.Client.Internal;
using request = Caerus.Client.Models.Request;
using common = Caerus.Common.Models.Segmentation;

namespace Boilerplate.Services.Thing;

public class ThingService : IThingService
{
    private readonly ICaerusClient _caerusClient;
    private readonly IInternalClient _caerusInternalClient;
    
    public ThingService(ICaerusClient caerusClient, IInternalClient caerusInternalClient)
    {
        _caerusClient = caerusClient;
        _caerusInternalClient = caerusInternalClient;
    }

    public async Task TestStuffHere()
    {
        while (true)
        {
            //break here and do something 
            await Task.CompletedTask;
        }
    }
}