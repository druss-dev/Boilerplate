using Boilerplate.Extensions;
using Boilerplate.Services.Thing;
using Boilerplate.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfigurationOptions();
builder.AddLogging();
builder.AddInternalServices();

var application = builder.Build();

// use this when accessing encrypted columns
//await application.AddEncryptionProvider();

await application.StartAsync();

var thingService = application.Services.GetRequiredService<IThingService>();

var thing = thingService.GetTheThing();
Console.WriteLine(thing.Result.StringyThing);