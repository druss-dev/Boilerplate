using Boilerplate.Models;

namespace Boilerplate.Services.Thing;

public interface IThingService
{
    Task<Models.Thing> GetTheThing();
}