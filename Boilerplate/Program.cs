using Boilerplate.Extensions;
using Boilerplate.Services.Thing;
using Boilerplate.Utilities;
using Microsoft.Extensions.Hosting;

var builder = HostBuilderExtensions.CreateHostBuilder(args);
var application = builder.Build();

// gets instance of a service
var thingServiceType = Type.GetType(ServiceType.ThingService);
var thingService = (ThingService)Activator.CreateInstance(thingServiceType!)!;
Console.WriteLine(thingService.GetTheThing().Result.StringyThing);

// use this when accessing encrypted columns
//await application.AddEncryptionProvider();

// need this is the console needs to keep running
//await application.RunAsync();