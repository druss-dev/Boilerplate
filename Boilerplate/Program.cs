using Boilerplate.Extensions;
using Boilerplate.Services.Thing;
using Boilerplate.Utilities;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfigurationOptions();
builder.AddLogging();
builder.AddInternalServices();

//var application = builder.Build();

// gets instance of a service
var thingServiceType = Type.GetType(ServiceType.ThingService);
var thingService = (ThingService)Activator.CreateInstance(thingServiceType!)!;
Console.WriteLine(thingService.GetTheThing().Result.StringyThing);

// use this when accessing encrypted columns
//await application.AddEncryptionProvider();

// need this is the console needs to keep running
//await application.RunAsync();