using Boilerplate.Extensions;
using Boilerplate.Models;
using Boilerplate.Services.Patient;
using Boilerplate.Utilities;
using Microsoft.Extensions.DependencyInjection;

var builder = HostBuilderExtensions.CreateHostBuilder(args);
var application = builder.Build();

// use this when accessing encrypted columns
await application.AddEncryptionProvider();

// gets instance of thing service (apparently only works for parameterless constructor)
// var thingServiceType = Type.GetType(ServiceType.ThingService);
// var thingService = (ThingService)Activator.CreateInstance(thingServiceType!)!;
// Console.WriteLine(thingService.GetTheThing().Result.StringyThing);

// gets instance of patient service
//var patientServiceType = Type.GetType(ServiceType.PatientService);
//var patientService = (PatientService)Activator.CreateInstance(patientServiceType!)!;

// need this is the console needs to keep running
await application.StartAsync();

var patientService = application.Services.GetRequiredService<IPatientService>();

var patientfile = new PatientFile
{
    ClientId = "b63bab43-ef1f-4f41-8689-633c5e88d21d",
    SegmentationGrouping = Enumerations.ServiceLineSegmentation.Heart
};

await patientService.ProcessPatientsAsync(patientfile);
