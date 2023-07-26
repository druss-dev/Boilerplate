using Boilerplate.Extensions;
using Boilerplate.Services.Thing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfigurationOptions();
builder.AddLogging();
builder.AddInternalServices();
builder.AddCaerusClient();

var application = builder.Build();

// use this when accessing encrypted columns
//await application.AddEncryptionProvider();

await application.StartAsync();

var thingService = application.Services.GetRequiredService<IThingService>();

try
{
    await thingService.TestStuffHere();
}
catch (Exception ex)
{
    //break here in case of exception
    Console.WriteLine("what happened", ex);
}