namespace Boilerplate.Services.Thing;

public class ThingService : IThingService
{
    // this is where we fetch something from the database or do some logic
    public Task<Models.Thing> GetTheThing()
    {
        var thing = new Models.Thing
        {
            StringyThing = "I am the thing we call string."
        };

        return Task.FromResult(thing);
    }
}