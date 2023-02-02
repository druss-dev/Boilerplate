using Boilerplate.Models;

namespace Boilerplate.Repositories.Thingabase;

public interface IThingabaseRepository
{
    Task GetThingFromDatabase(Thing thing);
}